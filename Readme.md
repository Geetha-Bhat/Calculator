# Calculator App
This application consists of following components
- API - This consists of single endpoint called Calculator with only Post endpoint
- API.Tests - Contains unit tests for the Calculator endpoint
- Models - Contains any classes that is shared between projects
- UI - User facing website

## Instructions on how to run
- Both API and UI project needs to be running for the application to work.

## Notes
- All the projects use default .Net Core templates (including Logging)
- Caching (in-memory cache) has been applied, so that memoisation of calculation and not waste resource

## Enhancements
- Apply authentication and authorisation on both API and front end.
- Enhance the Calculator logic to support more (modulus, brackets, square root etc)
- Allow admin staff to view all the items in the Cache, with the ability to remove single/all items