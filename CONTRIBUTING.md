# Contributing to Order Up

Thank you for your interest in contributing to Order Up! This document provides guidelines and instructions for contributing to the project.

## 📋 Table of Contents

- [Code of Conduct](#code-of-conduct)
- [Getting Started](#getting-started)
- [Development Workflow](#development-workflow)
- [Branch Naming Conventions](#branch-naming-conventions)
- [Commit Message Rules](#commit-message-rules)
- [Pull Request Process](#pull-request-process)
- [Coding Standards](#coding-standards)
- [Testing](#testing)

## 🤝 Code of Conduct

- Be respectful and inclusive
- Provide constructive feedback
- Focus on what is best for the project and community
- Show empathy towards other community members

## 🚀 Getting Started

1. **Fork the repository** on GitHub
2. **Clone your fork** locally:
   ```bash
   git clone https://github.com/YOUR-USERNAME/OrderUp.git
   cd OrderUp
   ```
3. **Add upstream remote**:
   ```bash
   git remote add upstream https://github.com/gavelinrobert-beep/OrderUp.git
   ```
4. **Open the project in Unity** (2021.3 LTS or newer)
5. **Install Mirror** networking package

## 🔄 Development Workflow

1. **Sync with upstream** before starting new work:
   ```bash
   git checkout main
   git pull upstream main
   git push origin main
   ```

2. **Create a new branch** for your feature/fix (see naming conventions below)

3. **Make your changes** following the coding standards

4. **Test your changes** thoroughly

5. **Commit your changes** following the commit message rules

6. **Push to your fork** and create a Pull Request

## 🌿 Branch Naming Conventions

Use the following prefixes for branch names:

### Branch Prefixes

- `feature/` - New features or enhancements
  - Example: `feature/picker-role-mechanics`
  - Example: `feature/express-orders`

- `fix/` - Bug fixes
  - Example: `fix/cart-collision-bug`
  - Example: `fix/score-calculation`

- `refactor/` - Code refactoring without changing functionality
  - Example: `refactor/order-system`
  - Example: `refactor/network-sync`

- `docs/` - Documentation updates
  - Example: `docs/api-documentation`
  - Example: `docs/setup-guide`

- `test/` - Adding or updating tests
  - Example: `test/picker-mechanics`
  - Example: `test/multiplayer-sync`

- `chore/` - Maintenance tasks, dependency updates
  - Example: `chore/update-mirror`
  - Example: `chore/cleanup-assets`

- `hotfix/` - Urgent fixes for production issues
  - Example: `hotfix/connection-crash`

### Branch Naming Format

```
<prefix>/<short-description>
```

**Rules:**
- Use lowercase letters
- Use hyphens to separate words
- Keep descriptions short but descriptive
- Avoid special characters except hyphens
- Maximum 50 characters recommended

**Examples:**
```
feature/packing-station-ui
fix/multiplayer-desync
refactor/item-spawner
docs/contributing-guidelines
test/order-generation
```

## 📝 Commit Message Rules

We follow the [Conventional Commits](https://www.conventionalcommits.org/) specification.

### Commit Message Format

```
<type>(<scope>): <subject>

<body>

<footer>
```

### Type

Must be one of the following:

- `feat`: New feature
- `fix`: Bug fix
- `docs`: Documentation changes
- `style`: Code style changes (formatting, missing semi-colons, etc.)
- `refactor`: Code refactoring
- `perf`: Performance improvements
- `test`: Adding or updating tests
- `chore`: Maintenance tasks, dependency updates
- `build`: Build system or external dependencies
- `ci`: CI/CD configuration changes
- `revert`: Reverting a previous commit

### Scope (Optional)

The scope should specify the area of the codebase affected:
- `picker` - Picker role mechanics
- `packer` - Packer role mechanics
- `orders` - Order system
- `ui` - User interface
- `network` - Networking/multiplayer
- `score` - Scoring system
- `cart` - Cart mechanics
- `map` - Map/environment

### Subject

- Use imperative, present tense: "add" not "added" or "adds"
- Don't capitalize first letter
- No period (.) at the end
- Maximum 50 characters

### Body (Optional)

- Use imperative, present tense
- Include motivation for the change
- Explain what and why, not how
- Wrap at 72 characters

### Footer (Optional)

- Reference issues: `Fixes #123`, `Closes #456`
- Note breaking changes: `BREAKING CHANGE: <description>`

### Examples

**Simple commit:**
```
feat(picker): add item pickup interaction
```

**Commit with scope and description:**
```
fix(network): resolve player position desync

Players were experiencing position desynchronization when picking up
items. This fixes the issue by ensuring all position updates are sent
through the network manager.

Fixes #42
```

**Commit with breaking change:**
```
refactor(orders)!: change order data structure

BREAKING CHANGE: Order class now uses a list instead of array for items.
This improves flexibility but requires updating all order-related code.
```

**Multiple types of commits:**
```
feat(ui): add order board visual feedback
fix(cart): correct collision detection
docs: update setup instructions
test(score): add unit tests for scoring system
chore: update Mirror to version 89.3.0
```

### Commit Message Rules Summary

✅ **DO:**
- Write clear, descriptive commit messages
- Use present tense ("add feature" not "added feature")
- Reference issue numbers when applicable
- Keep subject line under 50 characters
- Separate subject from body with blank line
- Wrap body at 72 characters

❌ **DON'T:**
- Write vague messages like "fix bug" or "update code"
- Use past tense
- Capitalize subject line
- End subject line with period
- Include multiple unrelated changes in one commit

## 🔀 Pull Request Process

1. **Update your branch** with the latest changes from main:
   ```bash
   git fetch upstream
   git rebase upstream/main
   ```

2. **Ensure your code**:
   - Follows the coding standards
   - Includes appropriate tests
   - Doesn't break existing functionality
   - Is properly documented

3. **Create a Pull Request** with:
   - Clear title following commit message format
   - Detailed description of changes
   - Screenshots/videos for UI changes
   - Reference to related issues

4. **PR Description Template**:
   ```markdown
   ## Description
   Brief description of changes
   
   ## Type of Change
   - [ ] Bug fix
   - [ ] New feature
   - [ ] Breaking change
   - [ ] Documentation update
   
   ## Testing
   Description of testing performed
   
   ## Screenshots (if applicable)
   
   ## Related Issues
   Fixes #(issue number)
   ```

5. **Code Review**:
   - Address reviewer feedback
   - Keep discussion focused and professional
   - Update PR as needed

6. **Merging**:
   - PRs require at least one approval
   - All CI checks must pass
   - Squash commits when merging (if requested)

## 💻 Coding Standards

### C# Style Guide

- Follow [Microsoft C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- Use PascalCase for public members
- Use camelCase for private fields with underscore prefix: `_privateField`
- Use meaningful variable names
- Add XML documentation comments for public APIs
- Keep methods focused and small (< 50 lines recommended)

### Unity Specific

- Organize scripts in appropriate folders (Scripts/Player, Scripts/Orders, etc.)
- Use SerializeField for inspector-visible private fields
- Cache component references in Awake/Start
- Avoid expensive operations in Update()
- Use object pooling for frequently instantiated objects
- Follow Unity naming conventions for MonoBehaviour methods

### Code Organization

```
Assets/
├── Scripts/
│   ├── Player/
│   ├── Orders/
│   ├── Networking/
│   ├── UI/
│   └── Utilities/
├── Prefabs/
├── Scenes/
├── Materials/
├── Textures/
└── Audio/
```

## 🧪 Testing

- Write unit tests for game logic
- Test multiplayer functionality with multiple clients
- Verify changes don't break existing features
- Test on different platforms when possible
- Include edge cases in testing

### Running Tests

```bash
# Run Unity Test Runner from Unity Editor
# Window > General > Test Runner
```

## 📞 Questions?

If you have questions about contributing:
1. Check existing documentation
2. Search closed issues for similar questions
3. Open a new issue with the "question" label

## 🙏 Thank You!

Your contributions make Order Up better for everyone. We appreciate your time and effort!

---

**Happy Coding! 🎮**
