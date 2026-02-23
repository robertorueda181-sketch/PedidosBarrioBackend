# Copilot Instructions

## General Guidelines
- First general instruction
- Second general instruction

## API Endpoints
- The endpoint `/api/Clientes/Auth/GoogleAuth` must function for both login and registration automatically.
  - Returns HTTP 201 if the client is new, and HTTP 200 if the client already exists.
  - The field `IsNewClient` in the response indicates this (true = new, false = existing).