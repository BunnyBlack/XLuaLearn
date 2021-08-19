#!/usr/bin/env python
# -*-coding:utf-8 -*-

"""
@File: res_diff_pack_generator.py
@Author: Yunyi Xu
@Date: 2021/8/12 10:35
@Desc: 生成差异包
"""
import os
import shutil
import xml.dom.minidom as xml_doc

# 生成差异包的版本跨度
import zipfile

version_span = 1
current_md5_dic = {}


def parse_version_md5(current_version: int, release_path: str, dic: dict):
    version_file_path = "{0}/MD5_ver{1}.xml".format(release_path, current_version)
    root: xml_doc.Element = xml_doc.parse(version_file_path).documentElement
    md5_configs = root.getElementsByTagName("file")
    config: xml_doc.Element
    for config in md5_configs:
        bundle_name = config.getAttribute("bundle_name")
        md5 = config.getAttribute("md5")
        dic[bundle_name] = md5


def do_generate_diff_pack(current_version: int, output_path: str, release_path: str):
    global version_span
    start_version = current_version - version_span
    if start_version <= 0:
        start_version = 1
    for version in range(start_version, current_version):
        version_md5_dic = {}
        parse_version_md5(version, release_path, version_md5_dic)
        diff_list = []
        # 不删包 只找多的包和变更了的包
        for bundle_name, md5 in current_md5_dic.items():
            version_md5 = version_md5_dic.get(bundle_name)
            if version_md5 is None or version_md5 != md5:
                diff_list.append(bundle_name)
        if len(diff_list) != 0:
            zipName = "DiffPack_{0}to{1}.zip".format(version, current_version)
            with zipfile.ZipFile(zipName, 'w', zipfile.ZIP_DEFLATED) as f:
                for bundle_name in diff_list:
                    bundle_path = "{0}/{1}".format(output_path, bundle_name)
                    f.write(bundle_path, bundle_name)
                # fileIndex.xml 和 bundleDependencies 单独放进去
                f.write("{0}/fileIndex.xml".format(output_path), "fileIndex.xml")
                f.write("{0}/bundleDependencies.xml".format(output_path), "bundleDependencies.xml")
            if os.path.exists("{0}/{1}".format(release_path, zipName)):
                os.remove("{0}/{1}".format(release_path, zipName))
            shutil.move(zipName, release_path)
            print("Generate {0}".format(zipName))
            pass
        else:
            print("No diff between {0} and {1}".format(version, current_version))


def generate_res_diff_pack(current_version: int, output_path: str, release_path: str):
    parse_version_md5(current_version, release_path, current_md5_dic)
    do_generate_diff_pack(current_version, output_path, release_path)
