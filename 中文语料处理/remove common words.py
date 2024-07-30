# _*_ coding : utf_8 _*_
# @Time : 2024/7/24 16:56
# @Author : faj
# @File : remove common words
# @Project : 新词与语义挖掘
import csv
import re
# 读取常用词文件
with open(r'C:\Users\86139\Desktop\现代汉语词典.txt',encoding='GB2312',errors='replace') as file:
    words = set()
    for line in file:
# 找出被【】框住的词语
        matches = re.findall(r'【(.*?)】',line)
        words.update(matches)
# 读取统计的字符对并删除常用词
with open (r'C:\Users\86139\Desktop\字符对频度.csv',encoding='utf-8') as csv_in :
    with open (r'C:\Users\86139\Desktop\字符对频度 - 副本.csv' , 'w' ,newline="", encoding='utf-8') as csv_out:
        reader = csv.reader(csv_in)
        writer = csv.writer(csv_out)
        for row in reader:
            if row[0] not in words:
                writer.writerow(row)