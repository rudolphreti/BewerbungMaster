import logging
from selenium.webdriver.common.by import By
from selenium.common.exceptions import NoSuchElementException
from base_crawler import BaseCrawler
import time

class AMSCrawler(BaseCrawler):
    def __init__(self, chrome_driver_path):
        super().__init__(chrome_driver_path, 'ams_data.json')

    def extract_job_information(self, job_element):
        try:
            link_element = job_element.find_element(By.XPATH, './/div/div/div[1]/div[1]/sn-list-item-header/h2/a')
            company_element = job_element.find_element(By.XPATH, './/div/div/div[2]/div[1]/sn-list-item-left/ams-icon-value[1]/div/div[2]/span')
            return {
                "id": self.generate_uuid(),
                "URL": link_element.get_attribute('href'),
                "position": self.clean_job_title(link_element.text.strip()),
                "company": company_element.text.strip()
            }
        except NoSuchElementException:
            return None

    def crawl_jobs(self):
        all_jobs, page_number = [], 0
        while True:
            url = f'https://jobs.ams.at/public/emps/jobs?page={page_number}&query=tester&location=wien&JOB_OFFER_TYPE=SB_WKO&JOB_OFFER_TYPE=IJ&JOB_OFFER_TYPE=BA&WORKING_TIME=V&PERIOD=ALL&sortField=PERIOD'
            self.driver.get(url)
            time.sleep(2)
            if "error" in self.driver.current_url:
                break
            job_elements = self.driver.find_elements(By.XPATH, '/html/body/sn-root/main/sn-search-page/div/div[1]/div[4]/section/sn-job-cards/sn-list-container/div/div/sn-list-item-container')
            if not job_elements:
                break
            all_jobs.extend([job for job in (self.extract_job_information(elem) for elem in job_elements) if job])
            logging.info(f"Added {len(job_elements)} jobs from page {page_number}.")
            page_number += 1
        return all_jobs