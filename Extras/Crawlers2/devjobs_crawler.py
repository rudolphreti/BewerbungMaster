from selenium import webdriver
from selenium.webdriver.common.by import By
from selenium.webdriver.chrome.options import Options
from selenium.webdriver.chrome.service import Service
from selenium.common.exceptions import NoSuchElementException, TimeoutException
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC
from base_crawler import BaseCrawler
import time
import logging

class DevJobsCrawler(BaseCrawler):
    def __init__(self, chrome_driver_path):
        super().__init__(chrome_driver_path, 'devjobs_data.json')
        self.base_url = "https://devjobs.at/jobs/search?locations=wien-109166&text=junior&sort=relevance"

    def initialize_driver(self):
        chrome_options = Options()
        chrome_options.add_argument("--headless")
        chrome_options.add_argument("--disable-gpu")
        chrome_options.add_argument("--no-sandbox")
        chrome_options.add_argument("--disable-dev-shm-usage")
        chrome_options.add_argument("user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36")
        service = Service(self.chrome_driver_path)
        self.driver = webdriver.Chrome(service=service, options=chrome_options)

    def extract_job_information(self, job_element):
        try:
            job_title = job_element.find_element(By.CSS_SELECTOR, 'h2').text.strip()
            company_name = job_element.find_element(By.CSS_SELECTOR, 'div.ml-2 p').text.strip()
            job_url = job_element.get_attribute('href')
            
            logging.info(f"Extracted job: {job_title} at {company_name}")
            return {
                "id": self.generate_uuid(),
                "position": self.clean_job_title(job_title),
                "company": company_name,
                "url": job_url
            }
        except NoSuchElementException as e:
            logging.error(f"Error extracting job information: {str(e)}")
            return None

    def crawl_jobs(self):
        all_jobs, page_number = [], 1
        while True:
            url = f"{self.base_url}&{'distance=0' if page_number == 1 else f'page={page_number}'}"
            logging.info(f"Navigating to page {page_number}: {url}")
            self.driver.get(url)
            
            try:
                WebDriverWait(self.driver, 20).until(
                    EC.presence_of_element_located((By.CSS_SELECTOR, "ul.flex.w-full.flex-col.space-y-4 > li"))
                )
            except TimeoutException:
                logging.warning(f"Timeout waiting for job listings on page {page_number}. Assuming no more pages.")
                break
            
            job_elements = self.driver.find_elements(By.CSS_SELECTOR, "ul.flex.w-full.flex-col.space-y-4 > li > a")
            
            if not job_elements:
                logging.info(f"No job listings found on page {page_number}. Stopping.")
                break
            
            jobs_on_page = [job for job in (self.extract_job_information(elem) for elem in job_elements) if job]
            all_jobs.extend(jobs_on_page)
            logging.info(f"Scraped {len(jobs_on_page)} jobs from page {page_number}.")
            
            page_number += 1
            time.sleep(5)  # Wait between page loads to be respectful
        
        return all_jobs

    def run(self):
        try:
            self.initialize_driver()
            jobs = self.crawl_jobs()
            self.save_jobs_to_json(jobs)
        except Exception as e:
            logging.exception(f"An error occurred during scraping: {str(e)}")
        finally:
            if self.driver:
                self.driver.quit()