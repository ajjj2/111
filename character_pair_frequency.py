# _*_ coding : utf_8 _*_
# @Time : 2024/7/18 19:12
# @Author : faj
# @File : 1
# @Project : 新词与语义挖掘
import pandas as pd
import re
with open (r'C:\Users\86139\Desktop\内科.txt') as f:
    s = f.read()
s1 = re.findall(r'[\u4e00-\u9fff]',s)
char = {}
index = 1
coword = {}
original_word = {}
# 编号映射
for word in s1:
    if word not in char:
        char[word] = index
        index+=1
N = index-1
# 统计频率
for i in range(len(s1) - 1):
    a = char[s1[i]]
    b = char[s1[i + 1]]
    unique = a * N + b
    original_word[unique] = (s1[i],s1[i+1])
    if unique in coword:
        coword[unique] += 1
    else:
        coword[unique] = 1
# 若需要所有字符对的频度，添加未出现的字符对为0
for char1 in char:
    for char2 in char:
        unique = char[char1] * N + char[char2]
        if unique not in coword:
            coword[unique] = 0
            original_word[unique] = (char1, char2)
# 降序排列
sorted_coword = sorted(coword.items(), key=lambda x: x[1], reverse=True)
# 输出
for unique, count in sorted_coword:
    wordpair = original_word[unique]
    print(f'索引: {wordpair}, 频度: {count}')
# 存储
data = [{'字符对': ''.join(original_word[unique]), '频度': count} for unique, count in sorted_coword]
df = pd.DataFrame(data)
df.to_csv(r'C:\Users\86139\Desktop\字符对频度.csv', index=False, encoding='utf-8-sig')