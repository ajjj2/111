from dawg_python import CompletionDAWG
import csv

def build_dawg_from_file(filename):
    """从文件中读取单词并构建DAWG"""
    with open(filename, 'r', encoding='GB18030') as file:
        words = [line.strip() for line in file if line.strip()]
    # 这里创建DAWG时应直接将列表传递给构造函数
    return CompletionDAWG(words)

def count_words_in_text(text, dawg):
    """使用DAWG检索文本中的单词并计数"""
    words = text.split()
    word_count = {}
    for word in words:
        if word in dawg:  # 使用 in 关键字检查单词是否存在于DAWG中
            if word in word_count:
                word_count[word] += 1
            else:
                word_count[word] = 1
    return word_count

def save_counts_to_csv(word_counts, filename):
    """将单词计数保存到CSV文件"""
    with open(filename, 'w', newline='', encoding='utf-8') as csvfile:
        writer = csv.writer(csvfile)
        writer.writerow(['Word', 'Count'])
        for word, count in sorted(word_counts.items(), key=lambda item: item[1], reverse=True):
            writer.writerow([word, count])

def main():
    dictionary_path = r'C:\Users\86139\Desktop\现代汉语词典.txt'
    text_path = r'C:\Users\86139\Desktop\内科.txt'
    output_csv_path = r'C:\Users\86139\Desktop\word_counts.csv'

    # 构建DAWG
    dawg = build_dawg_from_file(dictionary_path)

    # 读取并处理文本
    with open(text_path, 'r', encoding='GBK') as file:
        text = file.read()

    # 计数
    word_counts = count_words_in_text(text, dawg)

    # 保存到CSV
    save_counts_to_csv(word_counts, output_csv_path)

    print(f"Completed. Word counts saved to {output_csv_path}")

if __name__ == '__main__':
    main()
