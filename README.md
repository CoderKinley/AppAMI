# DT Meter Data Collection App

## Overview
The **DT Meter Data Collection App** is a professional application developed for **Bhutan Power Corporation (BPC)** to streamline the collection of energy meter data from approximately **5000 Distribution Transformers** (DTs) across Bhutan. This app is designed to improve efficiency, accuracy, and accessibility in monitoring and managing energy consumption data.

## Features
- **Real-Time Data Collection**: Fetches energy meter readings via APIs and stores data securely.
- **API Integration**: Connects with Bhutan Power Corporation’s central system for seamless data exchange.
- **User Authentication**: Secure login system with role-based access.
- **Offline Mode**: Allows data collection even without internet connectivity, syncing when online.
- **Data Visualization**: Displays collected data in graphical and tabular formats for quick analysis.
- **Search & Filter**: Users can search for specific DTs and filter data based on different parameters.
- **Export & Reports**: Generate and export reports in CSV or PDF format for further analysis.

## Technologies Used
| Technology  | Purpose  |
|-------------|----------|
| **Flutter** | Cross-platform mobile app development |
| **Dart**    | Programming language for Flutter |
| **SQLite**  | Local database storage for offline access |
| **REST API** | Fetching real-time data from BPC’s central server |
| **Provider/BLoC** | State management for efficient UI updates |
| **Firebase** | Authentication & cloud storage |
| **Google Maps API** | Visual representation of DT locations |

## System Architecture
1. **User Authentication**
   - Secure login using Firebase authentication.
   - Role-based access control.
2. **Data Fetching & Storage**
   - API integration with Bhutan Power Corporation’s database.
   - Stores data in SQLite for offline access.
3. **Meter Data Collection**
   - Displays list of 5000+ transformers and allows selection for data entry.
   - Supports automatic and manual meter reading entry.
4. **Data Syncing**
   - Synchronizes offline-collected data with the central database upon connectivity restoration.
5. **Analytics & Reports**
   - Real-time visualization of energy meter readings.
   - Generating reports in multiple formats (CSV, PDF).

## API Endpoints
| Endpoint | Method | Description |
|----------|--------|-------------|
| `/auth/login` | POST | Authenticate user and return token |
| `/meters` | GET | Fetch list of DT meters |
| `/meters/{id}` | GET | Retrieve data of a specific meter |
| `/meters/update` | POST | Submit meter readings |
| `/sync` | POST | Synchronize offline data with the server |

## Installation & Setup
1. Clone the repository:
   ```sh
   git clone https://github.com/your-repo/dt-meter-app.git
   cd dt-meter-app
   ```
2. Install dependencies:
   ```sh
   flutter pub get
   ```
3. Run the application:
   ```sh
   flutter run
   ```

## Future Enhancements
- **AI-powered anomaly detection** for irregular consumption patterns.
- **Automated notifications & alerts** for abnormal energy usage.
- **Integration with GIS systems** for advanced mapping and tracking.
- **Multi-language support** to accommodate diverse users.

## Contributors
- **Bhutan Power Corporation (BPC)** - Project Sponsor
- **[Your Name/Company]** - Development & Implementation

## License
This project is proprietary software developed for Bhutan Power Corporation. Unauthorized distribution is prohibited.

---
For further inquiries, please contact: **[Your Email/Support Contact]**.

