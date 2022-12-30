// system 
using System;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using System.Collections.Specialized;



// ODA
using Teigha.Runtime;
using Teigha.DatabaseServices;
using Teigha.Geometry;

// Bricsys
using Bricscad.ApplicationServices;
using Bricscad.Runtime;
using Bricscad.EditorInput;
using Bricscad.Ribbon;

// com
//using BricscadDb;
//using BricscadApp;

// alias
using _AcRx = Teigha.Runtime;
using _AcAp = Bricscad.ApplicationServices;
using _AcDb = Teigha.DatabaseServices;
using _AcGe = Teigha.Geometry;
using _AcEd = Bricscad.EditorInput;
using _AcGi = Teigha.GraphicsInterface;
using _AcClr = Teigha.Colors;
using _AcWnd = Bricscad.Windows;
namespace CsStepToDim
{
    class CXdata
    {
        public static int? GetXrecordInt(ObjectId id, string key)
        {
            ResultBuffer resbuf1 = GetXrecord(id, key);
            if (resbuf1 != null)
            {
                TypedValue[] result1 = resbuf1.AsArray();
                return Convert.ToInt32(result1[0].Value.ToString(), 10);
            }
            return null;
        }

        public static double? GetXrecordDouble(ObjectId id, string key)
        {
           
                ResultBuffer resbuf1 = GetXrecord(id, key);
            if (resbuf1 != null)
            {
                TypedValue[] result1 = resbuf1.AsArray();
            return Convert.ToDouble(result1[0].Value.ToString());
            }
            return null;
        }

        public static string GetXrecordString(ObjectId id, string key)
        {
            ResultBuffer resbuf1 = GetXrecord(id, key);
            if (resbuf1 != null)
            {
                TypedValue[] result1 = resbuf1.AsArray();
            return result1[0].Value.ToString();
            }
            return null;
        }


        public static void SetXrecordInt(ObjectId id, string key, int Data)
        {
            ResultBuffer resbuf1 = new ResultBuffer(new TypedValue(70, Data));
            SetXrecord(id, key, resbuf1);

        }

        public static void SetXrecordString(ObjectId id, string key, string Data)
        {
            ResultBuffer resbuf1 = new ResultBuffer(new TypedValue(1, Data));
            SetXrecord(id, key, resbuf1);

        }

        public static void SetXrecordDouble(ObjectId id, string key, double Data)
        {
            ResultBuffer resbuf1 = new ResultBuffer(new TypedValue(40, Data));
            SetXrecord(id, key, resbuf1);

        }


        public static void SetXrecord(ObjectId id, string key, ResultBuffer resbuf)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                Entity ent = tr.GetObject(id, OpenMode.ForRead) as Entity;
                if (ent != null)
                {
                    ent.UpgradeOpen();
                    ent.CreateExtensionDictionary();
                    DBDictionary xDict = (DBDictionary)tr.GetObject(ent.ExtensionDictionary, OpenMode.ForWrite);
                    Xrecord xRec = new Xrecord();
                    xRec.Data = resbuf;
                    xDict.SetAt(key, xRec);
                    xRec.Dispose();
                    tr.AddNewlyCreatedDBObject(xRec, true);
                }
                tr.Commit();
            }
        }

        public static ResultBuffer GetXrecord(ObjectId id, string key)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            ResultBuffer result = new ResultBuffer();
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                Xrecord xRec = new Xrecord();
                Entity ent = tr.GetObject(id, OpenMode.ForRead, false) as Entity;
                if (ent != null)
                {
                    try
                    {
                        DBDictionary xDict = (DBDictionary)tr.GetObject(ent.ExtensionDictionary, OpenMode.ForRead, false);
                        xRec = (Xrecord)tr.GetObject(xDict.GetAt(key), OpenMode.ForRead, false);
                        //foreach (DBDictionaryEntry entry in xDict)
                        //{
                        //    entry.Key.ToString();
                        //}
                        return xRec.Data;
                    }
                    catch
                    {
                        return null;
                    }
                }
                else
                    return null;
            }
        }

        public static StringCollection GetXrecordKeys(ObjectId id)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            ResultBuffer result = new ResultBuffer();
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                Xrecord xRec = new Xrecord();
                Entity ent = tr.GetObject(id, OpenMode.ForRead, false) as Entity;
                if (ent != null)
                {
                    try
                    {
                        DBDictionary xDict = (DBDictionary)tr.GetObject(ent.ExtensionDictionary, OpenMode.ForRead, false);
                        StringCollection strings = new StringCollection();
                        foreach (DBDictionaryEntry entry in xDict)
                        {
                            strings.Add( entry.Key.ToString());
                        }
                        return strings;
                    }
                    catch
                    {
                        return null;
                    }
                }
                else
                    return null;
            }
        }

        public static void RemoveXrecords(ObjectId id)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                Entity ent = tr.GetObject(id, OpenMode.ForRead) as Entity;
                if (ent != null)
                {
                    try
                    {
                        DBDictionary xDict = (DBDictionary)tr.GetObject(ent.ExtensionDictionary, OpenMode.ForRead, false);
                        if (xDict != null)
                            xDict.UpgradeOpen();
                        foreach (DBDictionaryEntry entry in xDict)
                            {
                                xDict.Remove(entry.Key.ToString());
                            }
                        xDict.Dispose();




                    }
                    catch
                    {
                       
                    }

                }
                tr.Commit();
            }
        }





    }

}