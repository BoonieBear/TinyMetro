using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Windows.Forms;
namespace TinyMetroWpfLibrary.Utility
{

    public class XmlHelper
    {
        #region 公共变量
        XmlDocument xmldoc;
        XmlNode xmlnode;
        XmlElement xmlelem;
        #endregion

        #region 创建Xml文档
        /// <summary>   
        /// 创建一个带有根节点的Xml文件   
        /// </summary>   
        /// <param name="FileName">Xml文件名称</param>   
        /// <param name="rootName">根节点名称</param>   
        /// <param name="Encode">编码方式:gb2312，UTF-8等常见的</param>   
        /// <param name="DirPath">保存的目录路径</param>   
        /// <returns></returns>   
        public static bool CreateXmlDocument(string FileName, string rootName, string Encode,string DirPath)
        {
            try
            {
                XmlDocument xmldoc = new XmlDocument();
                XmlDeclaration xmldecl;
                xmldecl = xmldoc.CreateXmlDeclaration("1.0", Encode, null);
                xmldoc.AppendChild(xmldecl);
                XmlElement xmlelem = xmldoc.CreateElement("", rootName, "");
                xmldoc.AppendChild(xmlelem);
                xmldoc.Save(DirPath+ Path.DirectorySeparatorChar.ToString()+ FileName);
                return true;
            }
            catch (Exception e)
            {
                return false;
                throw new Exception(e.Message);
            }
        }

        #endregion

        #region 常用操作方法(增删改)
        /// <summary>   
        /// 插入一个节点和它的若干子节点   
        /// </summary>   
        /// <param name="XmlFile">Xml文件路径</param>   
        /// <param name="NewNodeName">插入的节点名称</param>   
        /// <param name="HasAttributes">此节点是否具有属性，True为有，False为无</param>   
        /// <param name="fatherNode">此插入节点的父节点</param>   
        /// <param name="htAtt">此节点的属性，Key为属性名，Value为属性值</param>   
        /// <param name="htSubNode">子节点的属性，Key为Name,Value为InnerText</param>   
        /// <returns>返回真为更新成功，否则失败</returns>   
        public static bool InsertNode(string XmlFile, string NewNodeName, bool HasAttributes, string fatherNode, Hashtable htAtt, Hashtable htSubNode)
        {
            try
            {
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.Load(XmlFile);
                XmlNode root = xmldoc.SelectSingleNode(fatherNode);
                XmlElement xmlelem = xmldoc.CreateElement(NewNodeName);

                if (htAtt != null && HasAttributes)//若此节点有属性，则先添加属性   
                {
                    SetAttributes(xmlelem, htAtt);

                    SetNodes(xmlelem.Name, xmldoc, xmlelem, htSubNode);//添加完此节点属性后，再添加它的子节点和它们的InnerText   

                }
                else
                {
                    SetNodes(xmlelem.Name, xmldoc, xmlelem, htSubNode);//若此节点无属性，那么直接添加它的子节点   
                }

                root.AppendChild(xmlelem);
                xmldoc.Save(XmlFile);

                return true;
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);

            }
        }
        /// <summary>   
        /// 更新节点   
        /// </summary>   
        /// <param name="XmlFile">Xml文件路径</param>   
        /// <param name="fatherNode">需要更新节点的上级节点</param>   
        /// <param name="htAtt">需要更新的属性表，Key代表需要更新的属性，Value代表更新后的值</param>   
        /// <param name="htSubNode">需要更新的子节点的属性表，Key代表需要更新的子节点名字Name,Value代表更新后的值InnerText</param>   
        /// <returns>返回真为更新成功，否则失败</returns>   
        public static bool UpdateNode(string XmlFile, string fatherNode, Hashtable htAtt, Hashtable htSubNode)
        {
            try
            {
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.Load(XmlFile);
                XmlNodeList root = xmldoc.SelectSingleNode(fatherNode).ChildNodes;
                UpdateNodes(root, htAtt, htSubNode);
                xmldoc.Save(XmlFile);
                return true;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>   
        /// 删除指定节点下的子节点   
        /// </summary>   
        /// <param name="XmlFile">Xml文件路径</param>   
        /// <param name="fatherNode">制定节点</param>   
        /// <returns>返回真为更新成功，否则失败</returns>   
        public static bool DeleteNodes(string XmlFile, string fatherNode)
        {
            try
            {
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.Load(XmlFile);
                XmlNode xmlnode = xmldoc.SelectSingleNode(fatherNode);
                xmlnode.RemoveAll();
                xmldoc.Save(XmlFile);
                return true;
            }
            catch (XmlException xe)
            {
                throw new XmlException(xe.Message);
            }
        }
        #endregion

        #region 查找/修改值
        /// <summary>
        /// 各级输入的各级节点名称查找需要的值
        /// </summary>
        /// <param name="xmldoc">xml文件名</param>
        /// <param name="nodename">各级节点名称</param>
        /// <returns>配置值</returns>
        public static string GetConfigValue(string xmldoc, string[] nodename)
        {
            try
            {
                if (!File.Exists(xmldoc))
                    return null;
                XmlDocument xmlfile = new XmlDocument();
                xmlfile.Load(xmldoc);
                XmlNode xn = xmlfile.DocumentElement;
                foreach (string str in nodename)
                {
                    xn = xn.SelectSingleNode("descendant::" + str);
                }
                return xn.InnerText;
            }
            catch (Exception MyEx)
            {
                //MessageBox.Show(MyEx.StackTrace.ToString());
                //没有要找的内容
                Debug.WriteLine(MyEx.StackTrace);
                return null;
            }
        }

        /// <summary>
        /// 各级输入的各级节点名称修改需要的值
        /// </summary>
        /// <param name="xmldoc">xml文件名</param>
        /// <param name="nodename">各级节点名称</param>
        /// <returns>配置值</returns>
        public static bool SetConfigValue(string xmldoc, string[] nodename,string value)
        {
            try
            {
                if (!File.Exists(xmldoc))
                    return false;
                XmlDocument xmlfile = new XmlDocument();
                xmlfile.Load(xmldoc);
                XmlNode xn = xmlfile.DocumentElement;
                foreach (string str in nodename)
                {
                    xn = xn.SelectSingleNode("descendant::" + str);
                }
                if (xn != null)
                {
                    xn.InnerText = value;
                    xmlfile.Save(xmldoc);
                    return true;
                }
                return false;
            }
            catch (Exception MyEx)
            {
                //MessageBox.Show(MyEx.StackTrace.ToString());
                //没有要找的内容
                Debug.WriteLine(MyEx.StackTrace);
                return false;
            }
        }
        #endregion

        #region 私有方法
        /// <summary>   
        /// 设置节点属性   
        /// </summary>   
        /// <param name="xe">节点所处的Element</param>   
        /// <param name="htAttribute">节点属性，Key代表属性名称，Value代表属性值</param>   
        private static void SetAttributes(XmlElement xe, Hashtable htAttribute)
        {
            foreach (DictionaryEntry de in htAttribute)
            {
                xe.SetAttribute(de.Key.ToString(), de.Value.ToString());
            }
        }
        /// <summary>   
        /// 增加子节点到根节点下   
        /// </summary>   
        /// <param name="rootNode">上级节点名称</param>   
        /// <param name="XmlDoc">Xml文档</param>   
        /// <param name="rootXe">父根节点所属的Element</param>   
        /// <param name="SubNodes">子节点属性，Key为Name值，Value为InnerText值</param>   
        private static void SetNodes(string rootNode, XmlDocument XmlDoc, XmlElement rootXe, Hashtable SubNodes)
        {
            foreach (DictionaryEntry de in SubNodes)
            {
                XmlNode xmlnode = XmlDoc.SelectSingleNode(rootNode);
                XmlElement subNode = XmlDoc.CreateElement(de.Key.ToString());
                subNode.InnerText = de.Value.ToString();
                rootXe.AppendChild(subNode);
            }
        }
        /// <summary>   
        /// 更新节点属性和子节点InnerText值   
        /// </summary>   
        /// <param name="root">根节点名字</param>   
        /// <param name="htAtt">需要更改的属性名称和值</param>   
        /// <param name="htSubNode">需要更改InnerText的子节点名字和值</param>   
        private static void UpdateNodes(XmlNodeList root, Hashtable htAtt, Hashtable htSubNode)
        {
            foreach (XmlNode xn in root)
            {
                XmlElement xmlelem = (XmlElement)xn;
                if (xmlelem.HasAttributes)//如果节点如属性，则先更改它的属性   
                {
                    foreach (DictionaryEntry de in htAtt)//遍历属性哈希表   
                    {
                        if (xmlelem.HasAttribute(de.Key.ToString()))//如果节点有需要更改的属性   
                        {
                            xmlelem.SetAttribute(de.Key.ToString(), de.Value.ToString());//则把哈希表中相应的值Value赋给此属性Key   
                        }
                    }
                }
                if (xmlelem.HasChildNodes)//如果有子节点，则修改其子节点的InnerText   
                {
                    XmlNodeList xnl = xmlelem.ChildNodes;
                    foreach (XmlNode xn1 in xnl)
                    {
                        XmlElement xe = (XmlElement)xn1;
                        foreach (DictionaryEntry de in htSubNode)
                        {
                            if (xe.Name == de.Key.ToString())//htSubNode中的key存储了需要更改的节点名称，   
                            {
                                xe.InnerText = de.Value.ToString();//htSubNode中的Value存储了Key节点更新后的数据   
                            }
                        }
                    }
                }

            }
        }
        #endregion
    }
    
    /// <summary>
    /// 将xml文件导入到treeview里
    /// </summary>
    public class XmltoTree
    {
        public XmlDocument xDoc;
        public void populateTreeview(string xmldoc, TreeView tv)
        {
            try
            {
                //First, we'll load the Xml document
                xDoc = new XmlDocument();
                xDoc.Load(xmldoc);
                //Now, clear out the treeview, 
                //and add the first (root) node
                tv.Nodes.Clear();
                tv.Nodes.Add(new TreeNode(xDoc.DocumentElement.Name));
                TreeNode tNode = new TreeNode();
                tNode = (TreeNode)tv.Nodes[0];
                //We make a call to addTreeNode, 
                //where we'll add all of our nodes
                addTreeNode(xDoc.DocumentElement, tNode);
                //Expand the treeview to show all nodes
                tv.Nodes[0].Expand();
            }
            catch (XmlException xExc)
            //Exception is thrown is there is an error in the Xml
            {
                MessageBox.Show(xExc.Message);
            }
            catch (Exception ex) //General exception
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                
            }
            
        }
        //This function is called recursively until all nodes are loaded
        private void addTreeNode(XmlNode xmlNode, TreeNode treeNode)
        {
            XmlNode xNode;
            TreeNode tNode;
            XmlNodeList xNodeList;
            if (xmlNode.HasChildNodes) //The current node has children
            {
                xNodeList = xmlNode.ChildNodes;
                for (int x = 0; x <= xNodeList.Count - 1; x++)
                //Loop through the child nodes
                {
                    xNode = xmlNode.ChildNodes[x];
                    treeNode.Nodes.Add(new TreeNode(xNode.Name));
                    tNode = treeNode.Nodes[x];
                    addTreeNode(xNode, tNode);
                }
            }

            else //No children, so add the outer xml (trimming off whitespace)
            {
                treeNode.Text = xmlNode.OuterXml.Trim();
            }

                
        }
        public void exportToXml(TreeView tv, string filename)
        {
            using (XmlTextWriter xw = new XmlTextWriter(filename, System.Text.Encoding.UTF8))
            {
                xw.Formatting = Formatting.Indented;
                //Write our root node
                xw.WriteStartElement(tv.Nodes[0].Text);
                foreach (TreeNode node in tv.Nodes)
                {
                    saveNode(xw,node.Nodes);
                }
                //Close the root node
                xw.WriteEndElement();
            }
        }

        private void saveNode(XmlTextWriter sr, TreeNodeCollection tnc)
        {
            foreach (TreeNode node in tnc)
            {
                //If we have child nodes, we'll write 
                //a parent node, then iterrate through
                //the children
                if (node.GetNodeCount(true) > 1)
                {
                    sr.WriteStartElement(node.Text);
                    saveNode(sr, node.Nodes);
                    sr.WriteEndElement();
                }
                else //No child nodes, so we just write the text
                {
                    if (node.GetNodeCount(true) == 1)
                        sr.WriteElementString(node.Text, node.FirstNode.Text);
                    else
                        sr.WriteValue(node.Text);
                        //sr.WriteElementString(node.Text, null);
                }
            }
        }

    }

    /// <summary>
 
    
}
