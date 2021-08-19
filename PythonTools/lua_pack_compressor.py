#!/usr/bin/env python
# -*-coding:utf-8 -*-

"""
@File: lua_pack_compressor.py
@Author: Yunyi Xu
@Date: 2021/8/19 15:04
@Desc: 提取所有lua文件并打包
"""
import xml.dom.minidom as xml_doc
from hashlib import md5


def parse_lua_file_index(source_path: str):
    lua_index_full_path: str = source_path + "/luaIndex.xml"
    
    pass


def generate_version_lua_pack(source_path: str, lua_output_path: str, release_path: str):
    parse_lua_file_index(source_path)
    pass
