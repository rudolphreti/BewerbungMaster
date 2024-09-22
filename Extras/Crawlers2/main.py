import os
import shutil
from ams_crawler import AMSCrawler
from devjobs_crawler import DevJobsCrawler

def clean_job_data_folder():
    job_data_folder = os.path.join(os.path.dirname(os.path.abspath(__file__)), 'JobData')
    if os.path.exists(job_data_folder):
        for filename in os.listdir(job_data_folder):
            file_path = os.path.join(job_data_folder, filename)
            try:
                if os.path.isfile(file_path) or os.path.islink(file_path):
                    os.unlink(file_path)
                elif os.path.isdir(file_path):
                    shutil.rmtree(file_path)
            except Exception as e:
                print(f'Failed to delete {file_path}. Reason: {e}')
    else:
        os.makedirs(job_data_folder)
    print("JobData folder cleaned.")

def main():
    clean_job_data_folder()
    
    chrome_driver_path = 'D:/chromedriver-win64/chromedriver.exe'
    AMSCrawler(chrome_driver_path).run()
    DevJobsCrawler(chrome_driver_path).run()

if __name__ == "__main__":
    main()