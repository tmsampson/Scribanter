# Scribanter

Scribanter is a standalone tool which serves as a lightweight wrapper around [Scriban](https://github.com/scriban/scriban), designed to make static content generation from JSON models and [Scriban](https://github.com/scriban/scriban) templates simple and code-free. With Scribanter, you can:

- Generate static HTML pages (or any text-based content) directly from JSON data models.
- Batch process multiple templates and outputs
- Include reusable template components like headers and footers.
- Process single templates via command-line arguments or handle batch processing with a `.job` file.

## Why Scribanter?

Existing static site generators can be complex, requiring significant setup and coding. Scribanter reduces the boilerplate by leveraging [Scriban](https://github.com/scriban/scriban) and providing an easy-to-use CLI interface. All you need is:

- A `.json`  model containing your data. [Optional]
- A collection of `.scriban` [Scriban](https://github.com/scriban/scriban) templates (see [Syntax Guide](https://github.com/scriban/scriban/blob/master/doc/language.md)).
- A `.job` file to handle batch processing [Optional].

## Continuous Integration

![Build Status](https://github.com/tmsampson/Scribanter/actions/workflows/build.yml/badge.svg)
![Test Status](https://github.com/tmsampson/Scribanter/actions/workflows/unit-tests.yml/badge.svg)

## Getting Started

### Installation

You have two options for getting started:

#### Option 1: Download the binaries

1. Go to the [GitHub Releases](https://github.com/yourusername/scribanter/releases) page.
2. Download the latest `v1` binaries and extract them.

#### Option 2: Build from source

1. Clone the repository:

   ```bash
   git clone --recursive https://github.com/yourusername/scribanter.git
   cd scribanter
   ```

   Or, if you've already cloned without `--recursive`, run:

   ```bash
   git submodule update --init --recursive
   ```

2. For the best experience, open the repository using Visual Studio Code. This allows the project to be initialised, built and tested via pre-configured tasks.

3. Run the `init` task to set everything up.

4. Run the `build` task to compile the tool and tests.

### Usage

Scribanter uses a simple CLI interface:

```bash
scribanter --model path/to/model.json --template path/to/template.scriban --output path/to/output.html
```

### Examples

#### Basic Usage

Render a simple template:

```bash
scribanter --template path/to/template.scriban \
           --output path/to/output.txt
```

#### Using JSON Models

Feed a JSON model into the template:

```bash
scribanter --model path/to/animals.json \
           --template path/to/print_animals.scriban \
           --output path/to/print_animals.txt
```

#### Template Includes

Example of including templates and optionally passing in parameters:

```scriban
{{ include "Header.scriban" title: "My Page Title" }}

Some interesting content.

{{ include "Footer.scriban" }}
```

### Built-in Template Values

Scribanter provides a couple of built-in values you can access directly from within your templates:

```scriban
{{ TEMPLATE_PATH }}      # Path to the template file
{{ TEMPLATE_FILENAME }}  # Name of the template file with extension
{{ TEMPLATE_NAME }}      # Name of the template file without extension
```

### Path Usage

A few things to note regarding paths:

- Paths passed directly on commandline should be absolute, or relative to the CWD
- Paths used within job files should be absolute, or relative to the .job file
- Paths used in template includes should be absolute or relative to the root template

## Job Files (Optional)

For batch processing, use a `.job` file to describe inputs and outputs. Job files are executed as follows:

```bash
scribanter --job path/to/some.job
```

### Example 1

Single task, multiple items (no data model).

```json
{
 "tasks": [
  {
   "items": [
    {
     "template": "PrintCars.scriban",
     "output": "Output/Cars.txt"
    },
    {
     "template": "PrintAnimals.scriban",
     "output": "Output/Animals.txt"
    }
   ]
  }
 ]
}
```

### Example 2

Single task, multiple items, shared data model.

```json
{
 "tasks": [
  {
   "model": "CarsA.json",
   "items": [
    {
     "template": "PrintCars.scriban",
     "output": "Output/CarsA.txt"
    },
     {
     "template": "PrintCars.scriban",
     "output": "Output/CarsB.txt"
    }
   ]
  },
 ]
}
```

### Example 3

Multiple tasks (multiple data models).

```json
{
 "tasks": [
  {
   "model": "CarsA.json",
   "items": [
    {
     "template": "PrintCars.scriban",
     "output": "Output/CarsA.txt"
    }
   ]
  },
  {
   "model": "CarsB.json",
   "items": [
    {
     "template": "PrintCars.scriban",
     "output": "Output/CarsB.txt"
    }
   ]
  }
 ]
}
```

## Contributing

Feel free to fork the project and open pull requests with improvements or new features.

## License

MIT

---
Happy templating with Scribanter! 🚀
