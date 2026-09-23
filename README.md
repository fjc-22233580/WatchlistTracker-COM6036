# Watchlist Tracker

A web-based personal movie watchlist application developed for COM6036 Digital Innovation.

The application allows authenticated users to manage their own movie watchlist, track viewing status and ratings, search and filter saved movies, and retrieve movie metadata and poster images using The Movie Database (TMDB).

## Live Application

The deployed prototype is available at:

https://watchlist-com6036-app-p474q.ondigitalocean.app/

## Features

- User registration, login and logout
- User-specific watchlists
- Add, edit and delete movies
- Viewing status tracking
- Optional personal ratings
- Search and filtering
- TMDB movie search and metadata retrieval
- Poster image support
- Manual movie-entry option when TMDB is unavailable
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

Automated xUnit tests cover the main service-layer functionality, including:

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

Sensitive configuration values are not committed to source control. Full application access and local setup instructions are provided in the submitted report.

## Author

Francisco Castillo - 22233580  
COM6036 Digital Innovation
