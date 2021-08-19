#!/usr/bin/env python
# -*-coding:utf-8 -*-

"""
@File: lua_pack_compressor.py
@Author: Yunyi Xu
@Date: 2021/8/19 15:04
@Desc: 提取所有lua文件并打包
"""
import os
import shutil
import xml.dom.minidom as xml_doc
from hashlib import md5

lua_file_Dic = {}
current_lua_version: str


def parse_lua_file_index(source_path: str):
    lua_index_full_path: str = source_path + "/luaIndex.xml"
    root: xml_doc.Element = xml_doc.parse(lua_index_full_path).documentElement
    global current_lua_version
    current_lua_version = root.getAttribute("version")
    file_configs = root.getElementsByTagName("file")
    file_config: xml_doc.Element
    for file_config in file_configs:
        relative_path = file_config.getAttribute("relative_path")
        full_path = file_config.getAttribute("full_path")
        lua_file_Dic[relative_path] = full_path


def copy_file_to_lua_scripts_dir(lua_output_path: str):
    for relative_path, full_path in lua_file_Dic.items():
        dest_file = "{0}/{1}".format(lua_output_path, relative_path)
        dest_dir = os.path.dirname(dest_file)
        if not os.path.exists(dest_dir):
            os.mkdir(dest_dir)
        try:
            shutil.copyfile(full_path, dest_file)
        except IOError as e:
            print("Unable to copy file. %s" % e)

    print("File Copy Complete!")


def compress_files(lua_output_path, release_path):
    zipName = "LuaOriginBundle_ver{1}".format(lua_output_path, current_lua_version)
    shutil.make_archive(base_name="{0}/{1}".format(os.getcwd(), zipName), root_dir=lua_output_path, format="zip")
    if os.path.exists("{0}/{1}.zip".format(release_path, zipName)):
        os.remove("{0}/{1}.zip".format(release_path, zipName))
    shutil.move("{0}/{1}.zip".format(os.getcwd(), zipName), release_path)


def generate_all_lua_files_md5(lua_output_path, release_path):
    pass


def generate_version_lua_pack(source_path: str, lua_output_path: str, release_path: str):
    parse_lua_file_index(source_path)
    copy_file_to_lua_scripts_dir(lua_output_path)
    compress_files(lua_output_path, release_path)
    generate_all_lua_files_md5(lua_output_path, release_path)