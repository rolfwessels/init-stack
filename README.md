[![Github release](https://img.shields.io/github/v/release/rolfwessels/init-stack)](https://github.com/rolfwessels/init-stack/releases)
[![GitHub](https://img.shields.io/github/license/rolfwessels/init-stack)](https://github.com/rolfwessels/init-stack/blob/main/LICENSE.txt)

![Init stack](./docs/logo.png)

# 🌐 Init Stack

Streamline your project setup with intelligent templating. Init-Stack automates the creation of new software projects by cloning GitHub repository templates and intelligently renaming files and replacing identifiers throughout the source code to match your new project's name.

Built with .NET 9.0 and designed for cross-platform development.

## 📥 Installation

### Download Pre-built Binaries (Recommended)

Download the latest release for your platform from the [GitHub Releases page](https://github.com/rolfwessels/init-stack/releases/latest):

**Linux (x64):**
```bash
# Download and extract
wget https://github.com/rolfwessels/init-stack/releases/latest/download/init-stack-linux-x64-v*.zip
unzip init-stack-linux-x64-v*.zip
cd linux-x64

# Make executable and run
chmod +x init-stack
./init-stack new
```

**Windows (x64):**
1. Download `init-stack-win-x64-v*.zip` from the releases page
2. Extract the ZIP file
3. Run `init-stack.exe` from the extracted folder

**Verify Download (Optional):**
```bash
# Download checksums file
wget https://github.com/rolfwessels/init-stack/releases/latest/download/checksums.txt

# Verify the downloaded ZIP
sha256sum -c checksums.txt
```

### Build from Source

If you prefer to build from source, see the [Developer Setup](#-getting-started-as-a-developer) section below.

## 🔍 Using the CLI

### Quick Start

Create a new project from a template:

```bash
.\init-stack new
```

### Using a Local Template

Use a local folder as a template:

```bash
.\init-stack new c:\project\Template\MyprojectName
```

### Specify Output Name and Location

```bash
.\init-stack new c:\project\Template\MyprojectName MyNewName -o c:\Project\
```

### Using Template Names

If you know the template name:

```bash
.\init-stack new template-dotnet-core-console-app NewConsoleApp -o .
```

### Custom Git Configuration

The new project initializes with git and creates an initial commit. Override the git user and email:

```bash
.\init-stack new template-dotnet-core-console-app NewConsoleApp -o . --git-name "Your Name" --git-email "your.email@example.com"
```


## 🚀 Getting Started as a Developer

This project includes a development container with all required tooling for building, publishing, and deploying.

### Quick Setup

```bash
# Bring up the development environment
make build up

# Test the project
make test

# Run the application
make start

# Build for production
make publish
```

### Docker Deployment

```bash
# Build, login, and push Docker images
make docker-publish

# Tag a specific environment release
make docker-pull-short-tag docker-tag-env env=latest

# Deploy to an environment
make deploy env=dev
```
## 🛠 Prerequisites

- **Docker**: Required for containerized development. [Installation guide](https://docs.docker.com/get-docker/)
- **Git**: For version control. [Download Git](https://git-scm.com/downloads)
- **Make**: Command runner (available via WSL on Windows)
- **.NET 9.0 SDK**: For local development outside containers 

## 📋 Available Make Commands

### 💻 Local Machine Commands

- `make up`: Start the container and attach to the default shell
- `make down`: Stop the container
- `make build`: Rebuild the container

### 🐳 Container Commands

- `make version`: Display current version number
- `make start`: Run Init Stack
- `make test`: Run tests with coverage
- `make publish`: Build production artifacts
- `make package`: Package binaries into ZIP files (includes checksums)
- `make docker-login`: Authenticate with Docker registry
- `make docker-build`: Build Docker image
- `make docker-push`: Push Docker image to registry
- `make docker-pull-short-tag`: Pull image by git hash
- `make docker-tag-env`: Tag image for specific environment
- `make docker-publish`: Complete Docker build and publish workflow
- `make deploy`: Deploy to specified environment
- `make update-packages`: Update NuGet packages to latest versions

## 💻 Development

### Adding a New Project

```bash
# Create a new class library
dotnet new classlib -n InitStack.Core -o ./src/InitStack.Core

# Create corresponding test project
dotnet new nunit -n InitStack.Core.Tests -o ./tests/InitStack.Core.Tests

# Add to solution
dotnet sln InitStack.sln add ./src/InitStack.Core/InitStack.Core.csproj
dotnet sln InitStack.sln add ./tests/InitStack.Core.Tests/InitStack.Core.Tests.csproj

# Verify
dotnet sln InitStack.sln list
```

### 🔄 Versioning

This project follows [Semantic Versioning (SemVer)](https://semver.org/):

- **MAJOR**: Incompatible API changes
- **MINOR**: Backward-compatible functionality additions
- **PATCH**: Backward-compatible bug fixes

**Version Management:**

- `MAJOR` and `MINOR` versions are set manually via the `versionPrefix` variable in the Makefile
- `PATCH` levels are automatically determined from commit count on the `main` branch
- Feature branches include pre-release identifiers based on commit delta from `main`

**Check Current Version:**

```bash
make version
```

## 🔄 Development Workflow

This project uses a modified [Git Flow](https://nvie.com/posts/a-successful-git-branching-model/) workflow optimized for continuous delivery.

### Branching Strategy

**Feature/Bug Branches**

- Create from `main` branch
- Naming: `feature/description` or `bug/description`
- Examples: `feature/add-login`, `bug/fix-login-button`

**Main Branch**

- Production-ready code
- Feature branches merge here when complete
- CI/CD pipeline deploys to development environments

**Releases**

- Use **tags** instead of release branches
- Process:
  1. Ensure `main` branch is stable
  2. Merge all desired changes
  3. Create a version tag (e.g., `v1.2.0`)
  4. Tagged commit is the official release

**Hotfixes**

- Create from specific `v1.xxx` tag
- Merge back to `main`
- Create new patch tag (e.g., `v1.2.1`)

## ❓ FAQ

**Q: Is this application cross-platform?**

A: Yes, Init Stack runs on Linux, macOS, and Windows.

**Q: How do I update to the latest version?**

A: Pull the latest changes from the repository and rebuild your Docker container, or download the latest release from GitHub.

**Q: What templates are available?**

A: You can use any GitHub repository as a template, or create your own local templates.

## 📚 Resources

- [What is a Makefile?](https://opensource.com/article/18/8/what-how-makefile)
- [Git Flow Workflow](https://nvie.com/posts/a-successful-git-branching-model/)
- [Semantic Versioning](https://semver.org/)

## 📄 License

This project is licensed under the terms specified in [LICENSE.txt](LICENSE.txt).
