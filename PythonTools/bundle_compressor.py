#!/usr/bin/env python
# -*-coding:utf-8 -*-

"""
@File: bundle_compressor.py
@Author: Yunyi Xu
@Date: 2021/8/11 17:38
@Desc: 筛选有效ab包并打成压缩包
"""
import os
import shutil
import xml.dom.minidom as xml_doc
from hashlib import md5

bundle_list = []
current_version: str


def parse_file_index(source_path: str):
    file_index_full_path: str = source_path + "/fileIndex.xml"
    root: xml_doc.Element = xml_doc.parse(file_index_full_path).documentElement
    global current_version
    current_version = root.getAttribute("version")
    file_configs = root.getElementsByTagName("file")
    file_config: xml_doc.Element
    for file_config in file_configs:
        bundle_name = file_config.getAttribute("bundle_name")
        bundle_list.append(bundle_name)


def copy_file_to_output_dir(source_path: str, dest_path: str):
    for bundle_name in bundle_list:
        source_file = "{0}/{1}".format(source_path, bundle_name)
        dest_file = "{0}/{1}".format(dest_path, bundle_name)
        dest_dir = os.path.dirname(dest_file)
        if not os.path.exists(dest_dir):
            os.mkdir(dest_dir)
        try:
            shutil.copyfile(source_file, dest_file)
        except IOError as e:
            print("Unable to copy file. %s" % e)

    # 单独把fileIndex和bundleDependencies移动过去
    try:
        shutil.copyfile("{0}/fileIndex.xml".format(source_path), "{0}/fileIndex.xml".format(dest_path))
        shutil.copyfile("{0}/bundleDependencies.xml".format(source_path),
                        "{0}/bundleDependencies.xml".format(dest_path))
    except IOError as e:
        print("Unable to copy file. %s" % e)

    print("File Copy Complete!")


def compress_files(dest_path: str):
    zipName = "OriginBundle_ver{1}".format(dest_path, current_version)
    shutil.make_archive(base_name="{0}/{1}".format(os.getcwd(), zipName), root_dir=dest_path, format="zip")
    if os.path.exists("{0}/{1}.zip".format(dest_path, zipName)):
        os.remove("{0}/{1}.zip".format(dest_path, zipName))
    shutil.move("{0}/{1}.zip".format(os.getcwd(), zipName), dest_path)


def generate_all_bundles_md5(dest_path: str):
    doc = xml_doc.Document()
    root: xml_doc.Element = doc.createElement("root")
    doc.appendChild(root)
    for bundle_name in bundle_list:
        node: xml_doc.Element = doc.createElement("file")
        node.setAttribute("bundle_name", bundle_name)
        # 注意这里的update会拼接前后传入的内容 所以我们必须要重新创建新的md5对象才可以
        with open("{0}/{1}".format(dest_path, bundle_name), "rb") as file:
            contents = file.read()
            m = md5()
            m.update(contents)
            result = m.hexdigest()
            node.setAttribute("md5", result)
        root.appendChild(node)
    with open("{0}/MD5_ver{1}.xml".format(dest_path, current_version), "w+") as xml_file:
        doc.writexml(xml_file, indent='', addindent='\t', newl='\n', encoding='UTF-8')
    print("generate md5")


def generate_version_bundles(source_path: str, dest_path: str):
    parse_file_index(source_path)
    copy_file_to_output_dir(source_path, dest_path)
    compress_files(dest_path)
    generate_all_bundles_md5(dest_path)
