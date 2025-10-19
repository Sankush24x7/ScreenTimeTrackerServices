# ScreenTimeTrackerServices
ScreenTimeTrackerServices

📊 Screen Time Tracker – Modular Reporting System
This project is a modular, automated screen time tracking and reporting system designed to:

Log daily activity in encrypted form

Generate daily HTML reports and Excel summaries

Consolidate daily summaries into monthly reports

Email reports with embedded content and attachments

Clean up old logs and manage monthly rollovers

🔁 Daily Workflow
Track & Save Logs: Save encrypted logs to /Logs/Logs_yyyy-MM-dd.json

Generate Summary: Create EDailySummaryResult and save as summary_yyyyMMdd.json

Generate HTML Report: Save styled HTML report for email body

Append to Monthly Summary: Update monthly consolidated JSON with new summary

Send Email: Embed HTML in body, attach report

Cleanup: Delete previous day's log after successful email

🔐 Security
All logs and monthly summaries are AES-encrypted using Encryptor.Encrypt(...) and Encryptor.Decrypt(...)

Daily summaries and HTML reports can be optionally encrypted or left in plaintext for easier debugging

📬 Email Format
Subject: Screen Time Report - yyyy-MM-dd | System: MACHINE | Host: HOSTNAME

Body: Embedded HTML report

Attachments: Daily HTML report (and optionally Excel or monthly JSON)


**use below command to run the services of ScreenTimeTracker in each local service****using Administrator Command**
sc create ScreenTimeTrackerService binPath= "C:\ScreenTimeService\ScreenTimeTrackerServices.exe"
sc config ScreenTimeTrackerService start= auto
sc start ScreenTimeTrackerService
sc delete ScreenTimeTrackerService

