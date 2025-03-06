# FBMessenger2RAG

A C# utility for parsing and normalizing Facebook Messenger conversation data exported via the "Download Your Information" portal (https://www.facebook.com/dyi/). This tool processes JSON chat exports, extracting and structuring message content to prepare it for training Retrieval-Augmented Generation (RAG) models. It decodes Unicode-escaped text (e.g., Georgian characters), sorts conversations chronologically, and generates both JSON and TXT outputs for downstream AI applications.

## Features
- **Input**: Reads JSON files from a specified folder and its subfolders.
- **Normalization**: Converts Unicode escape sequences into readable text (e.g., Georgian script).
- **Extraction**: Captures `sender_name`, `content`, and `timestamp_ms` from messages.
- **Output**: 
  - Combined JSON file with structured message data.
  - TXT file with chronologically sorted `content`, separated by conversation breaks.
- **Sorting**: Orders messages by `timestamp_ms` for temporal accuracy.
- **Use Case**: Prepares clean, structured data for RAG model training or NLP tasks.

## Purpose
FBMessenger2RAG simplifies the preprocessing of raw Messenger JSON exports, transforming them into formats optimized for training RAG systems. Perfect for developers, researchers, or data scientists working with social media conversation data for AI projects.

## Getting Started
1. Clone the repo: `git clone https://github.com/moseshvili/FBMessenger2RAG.git`
2. Install dependencies: `Newtonsoft.Json` via NuGet.
3. Run the console app, provide a folder path, and process your Messenger JSON exports.

Contributions are encouraged! Check the issues tab for planned features or to report bugs.
