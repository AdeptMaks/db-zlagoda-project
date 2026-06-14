#!/bin/sh
cd "$(dirname "$0")"
docker compose --profile migrate run --build --rm migrate
