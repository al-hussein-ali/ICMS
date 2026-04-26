
import sys

def check_braces(file_path):
    balance = 0
    with open(file_path, 'r', encoding='utf-8') as f:
        for i, line in enumerate(f, 1):
            clean_line = line.split('//')[0]
            for char in clean_line:
                if char == '{':
                    balance += 1
                elif char == '}':
                    balance -= 1
            if balance == 0 and i > 23:
                print(f"LINE {i}: BALANCE 0")
            if balance < 0:
                print(f"LINE {i}: BALANCE NEGATIVE ({balance})")
    print(f"Final balance: {balance}")

if __name__ == "__main__":
    check_braces(sys.argv[1])
