# Watchlist Tracker

A web-based personal movie watchlist application developed for COM6036 Digital Innovation.

The application allows authenticated users to manage their own movie watchlist, track viewing status and ratings, search and filter saved movies, and retrieve movie metadata and poster images using The Movie Database (TMDB).

## Features

- User registration, login and logout
- Personal user-specific watchlists
- Add, edit and delete movies
- Viewing status tracking
- Optional personal ratings
- Search and filtering
- TMDB movie search and metadata retrieval
- Poster image support
- Manual movie-entry fallback
- Responsive Bootstrap interface

## Technology

- ASP.NET Core Razor Pages
- C# / .NET
- Entity Framework Core
- PostgreSQL
- Supabase
- ASP.NET Core Identity
- Bootstrap
- TMDB API
- xUnit

## Architecture

The application follows a layered N-tier structure separating:

- Presentation
- Application logic
- Data access

This separation supports maintainability, testability and clear responsibility boundaries.

## Testing

Automated unit tests cover the main service-layer functionality, including:

- Watchlist CRUD operations
- User-specific data access
- Search and filtering
- Dashboard counts
- TMDB search and metadata handling

Manual functional testing was also carried out against the defined functional and non-functional requirements.

## Repository Contents

This repository contains:

- Application source code
- Entity Framework Core migrations
- Automated tests

## Author

Francisco Castillo  
COM6036 Digital Innovation
