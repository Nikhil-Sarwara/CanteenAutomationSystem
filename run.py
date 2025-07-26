#!/usr/bin/env python3
import subprocess
import sys
import redis
import psycopg2
import re

# --- Configuration ---
# IMPORTANT: Fill in these placeholder values before running the script.
IMAGE_NAME = "canteen-api"
CONTAINER_NAME = "canteen-backend"

# Use 'host.docker.internal' for the Docker run command to connect from the container to your host machine.
# For direct connection tests from this script, use 'localhost'.
DB_HOST = "localhost"
DB_NAME = "CanteenDB"
DB_USER = "nikhilsarwara"
DB_PASSWORD = ""
REDIS_HOST = "localhost"
REDIS_PORT = 6379

# Connection strings for the Docker container
DOCKER_DB_CONNECTION = f"Host=host.docker.internal;Database={DB_NAME};Username={DB_USER};Password={DB_PASSWORD}"
DOCKER_REDIS_CONNECTION = "host.docker.internal:6379"

JWT_KEY = "A_VERY_SECRET_KEY_THAT_IS_LONG_ENOUGH"
JWT_ISSUER = "CanteenAutomationSystem"
JWT_AUDIENCE = "CanteenUsers"
# ---------------------

def test_db_connection():
    """Tests the connection to the PostgreSQL database."""
    print("\n--- Testing PostgreSQL Connection ---")
    try:
        conn = psycopg2.connect(
            host=DB_HOST,
            dbname=DB_NAME,
            user=DB_USER,
            password=DB_PASSWORD
        )
        conn.close()
        print("✅ PostgreSQL connection successful!")
    except psycopg2.Error as e:
        print("❌ PostgreSQL connection failed.")
        print(f"   Error: {e}")
    print("-" * 35)

def test_redis_connection():
    """Tests the connection to the Redis server."""
    print("\n--- Testing Redis Connection ---")
    try:
        r = redis.Redis(host=REDIS_HOST, port=REDIS_PORT, decode_responses=True)
        r.ping()
        print("✅ Redis connection successful!")
    except redis.exceptions.RedisError as e:
        print("❌ Redis connection failed.")
        print(f"   Error: {e}")
    print("-" * 30)

def build_docker_image():
    """Builds a Docker image with the given name."""
    print(f"\n--- Building Docker image '{IMAGE_NAME}' ---")
    try:
        subprocess.run(["docker", "build", "-t", IMAGE_NAME, "."], check=True, capture_output=True, text=True)
        print("\n✅ Docker image built successfully!")
    except subprocess.CalledProcessError as e:
        print("\n❌ Failed to build the Docker image.")
        print(f"   Error: {e.stderr}")
    print("-" * 45)

def run_redis_container():
    """Runs a Redis container."""
    print(f"\n--- Preparing to run Redis container ---")
    subprocess.run(["docker", "run", "-d", "-p", "6379:6379", "redis"], capture_output=True, text=True)

def run_container():
    """Stops, removes, and runs the Docker container."""
    print(f"\n--- Preparing to run container '{CONTAINER_NAME}' ---")

    # Stop and remove the existing container (ignore errors if it doesn't exist)
    subprocess.run(["docker", "stop", CONTAINER_NAME], capture_output=True, text=True)
    subprocess.run(["docker", "rm", CONTAINER_NAME], capture_output=True, text=True)

    print(f"Starting new container '{CONTAINER_NAME}'...")
    run_command_list = [
        "docker", "run", "-d", "-p", "8080:80",
        "-e", f"ConnectionStrings:DefaultConnection={DOCKER_DB_CONNECTION}",
        "-e", f"ConnectionStrings:Redis={DOCKER_REDIS_CONNECTION}",
        "-e", f"Jwt:Key={JWT_KEY}",
        "-e", f"Jwt:Issuer={JWT_ISSUER}",
        "-e", f"Jwt:Audience={JWT_AUDIENCE}",
        "--name", CONTAINER_NAME,
        IMAGE_NAME
    ]
    
    try:
        subprocess.run(run_command_list, check=True, capture_output=True, text=True)
        print("\n✅ Container started successfully!")
        print(f"   Your API should be accessible at http://localhost:8080")
    except subprocess.CalledProcessError as e:
        print("\n❌ Failed to start the container.")
        print(f"   Error: {e.stderr}")
    print("-" * 45)


def main():
    """Displays a menu and executes the chosen action."""
    while True:
        print("\n--- Canteen Automation Runner ---")
        print("1. Test Database Connection")
        print("2. Test Redis Connection")
        print("3. Build Docker Image")
        print("4. Run Docker Redis")
        print("5. Run Docker Container")
        print("6. Exit")
        
        choice = input("Enter your choice (1-6): ")
        
        if choice == '1':
            test_db_connection()
        elif choice == '2':
            test_redis_connection()
        elif choice == '3':
            build_docker_image()
        elif choice == '4':
            run_redis_container()
        elif choice == '5':
            run_container()
        elif choice == '6':
            print("Exiting.")
            sys.exit(0)
        else:
            print("Invalid choice. Please enter a number between 1 and 6.")

if __name__ == "__main__":
    main()

