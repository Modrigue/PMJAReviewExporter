import os
import re
import sys

# Path to AssemblyInfo.cs (relative to repo root)
ASSEMBLY_INFO_PATH = os.path.join(os.path.dirname(os.path.dirname(__file__)), 'Properties', 'AssemblyInfo.cs')

# Get the tag from environment variable set by GitHub Actions (GITHUB_REF or GITHUB_TAG)
GITHUB_REF = os.environ.get('GITHUB_REF', '')

def get_tag_from_ref(ref):
    # GitHub ref format: refs/tags/1.2.3.4
    if ref.startswith('refs/tags/'):
        return ref[len('refs/tags/'):]
    
    return None

def update_assembly_version(tag):
    pattern = re.compile(r'Version\(".*"\)')
    replacement = f'Version("{tag}")'

    with open(ASSEMBLY_INFO_PATH, 'r', encoding='utf-8') as f:
        content = f.read()

    # Replace version lines
    new_content, count = pattern.subn(replacement, content)
    if count > 0 and new_content != content:
        with open(ASSEMBLY_INFO_PATH, 'w', encoding='utf-8') as f:
            f.write(new_content)
        print(f"Updated AssemblyVersion to {tag}")

def main():
    tag = get_tag_from_ref(GITHUB_REF)
    if not tag:
        return
    
    update_assembly_version(tag)

if __name__ == '__main__':
    main() 