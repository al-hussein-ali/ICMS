
import sys

def check_all_brackets(file_path):
    stack = []
    brackets = {'{': '}', '(': ')', '[': ']'}
    with open(file_path, 'r', encoding='utf-8') as f:
        for i, line in enumerate(f, 1):
            clean_line = line.split('//')[0]
            for char in clean_line:
                if char in brackets:
                    stack.append((char, i))
                elif char in brackets.values():
                    if not stack:
                        print(f"Extra closing bracket {char} at line {i}")
                    else:
                        opening, start_line = stack.pop()
                        if brackets[opening] != char:
                            print(f"Mismatch: {opening} at line {start_line} closed by {char} at line {i}")
    while stack:
        opening, start_line = stack.pop()
        print(f"Unclosed bracket {opening} at line {start_line}")

if __name__ == "__main__":
    check_all_brackets(sys.argv[1])
