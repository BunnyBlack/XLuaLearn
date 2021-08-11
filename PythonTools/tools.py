#!/usr/bin/env python
# -*-coding:utf-8 -*-

"""
@File: tools.py
@Author: Yunyi Xu
@Date: 2021/8/11 17:27
@Desc: 打包用的 python 工具接口
"""
import os
import sys
import bundle_compressor as compressor


def generate_version_bundles(source_path: str, dest_path: str):
    if not os.path.exists(dest_path):
        os.mkdir(dest_path)
    compressor.generate_version_bundles(source_path, dest_path)


if __name__ == "__main__":
    eval(sys.argv[1])
