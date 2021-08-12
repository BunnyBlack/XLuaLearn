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
import diff_pack_generator


def generate_version_bundles(source_path: str, output_path: str, release_path: str):
    if not os.path.exists(output_path):
        os.mkdir(output_path)
    if not os.path.exists(release_path):
        os.mkdir(release_path)
    compressor.generate_version_bundles(source_path, output_path, release_path)


def generate_diff_pack(current_version: int, output_path: str, release_path: str):
    diff_pack_generator.generate_diff_pack(current_version, output_path, release_path)


if __name__ == "__main__":
    eval(sys.argv[1])
