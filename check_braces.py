
import sys

def check_braces(file_path, limit_line):
    balance = 0
    with open(file_path, 'r', encoding='utf-8') as f:
        for i, line in enumerate(f, 1):
            if i > limit_line:
                break
            # Remove string literals and comments to avoid false positives
            # This is a very simple parser, might be imperfect
            clean_line = line.split('//')[0] 
            for char in clean_line:
                if char == '{':
                    balance += 1
                elif char == '}':
                    balance -= 1
                if balance == 0 and i > 23:
                    print(f"FIRST BALANCE 0 REACHED AT LINE {i}")
                    sys.exit(0)
    print(f"Final balance at line {limit_line}: {balance}")

if __name__ == "__main__":
    check_braces(sys.argv[1], int(sys.argv[2]))
