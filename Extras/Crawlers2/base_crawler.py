import os
import re
import json
import uuid
import logging
from abc import ABC, abstractmethod
from selenium import webdriver
from selenium.webdriver.chrome.service import Service
from selenium.webdriver.chrome.options import Options

class BaseCrawler(ABC):
    def __init__(self, chrome_driver_path, output_file_name):
        self.chrome_driver_path = chrome_driver_path
        self.output_file_name = output_file_name
        self.driver = None
        self.setup_logging()
        self.compiled_regex = re.compile(self._get_regex_pattern(), re.VERBOSE | re.IGNORECASE)

    def setup_logging(self):
        logging.basicConfig(level=logging.INFO, format='%(asctime)s - %(levelname)s - %(message)s')

    def initialize_driver(self):
        chrome_options = Options()
        chrome_options.add_argument("--headless")
        service = Service(self.chrome_driver_path)
        self.driver = webdriver.Chrome(service=service, options=chrome_options)

    @staticmethod
    def _get_regex_pattern():
        # TODO: unreadable!!! add comments 
        # (w/m/div.) or (f/m/div*)
        # Full time
        # Start as of now
        # (mind. 30 Std/Woche)
        # :r
        # _
        # *
        # Salary
        # digits

        return r"""\((?:[dmfwx](/?[dmfwx])*(\s/\*)?|[dmf](?:\|[dmf]){1,2})\)|[:/\*]*in\b|\(all\s+genders\)|[:_]innen\b"""

    def generate_uuid(self):
        return str(uuid.uuid1())

    def clean_job_title(self, title):
        return self.compiled_regex.sub('', title)

    def save_jobs_to_json(self, jobs):
        # Get the directory of the main script
        root_dir = os.path.dirname(os.path.abspath(__file__))
        
        # Create JobData folder in the root directory
        job_data_folder = os.path.join(root_dir, 'JobData')
        os.makedirs(job_data_folder, exist_ok=True)
        
        # Construct the full path for the JSON file
        json_file_path = os.path.join(job_data_folder, self.output_file_name)
        
        with open(json_file_path, 'w', encoding='utf-8') as file:
            json.dump(jobs, file, ensure_ascii=False, indent=4)
        logging.info(f"Created {json_file_path} with {len(jobs)} jobs.")

    @abstractmethod
    def extract_job_information(self, job_element):
        pass

    @abstractmethod
    def crawl_jobs(self):
        pass

    def run(self):
        try:
            self.initialize_driver()
            jobs = self.crawl_jobs()
            self.save_jobs_to_json(jobs)
        except Exception as e:
            logging.exception(f"Error during crawling: {str(e)}")
        finally:
            if self.driver:
                self.driver.quit()