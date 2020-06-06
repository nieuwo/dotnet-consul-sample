# .NET Core 3 & Consul Service Discovery & Leader Election

A .NET Core Web API that registers itself with Consul on startup via Traefik and implements a leader election pattern for HA instnaces spread across DCs.
All components run in Docker.

* Consul dashboard is at http://localhost:8500/ui/eu_west/services
* Traefik dashboard is at http://localhost:8080/dashboard/

## Usage

Create containers:  
`docker-compose up -d`