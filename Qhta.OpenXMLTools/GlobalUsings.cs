// Global using directives

global using System.Collections.Generic;

global using System.Diagnostics;
global using System.Globalization;
global using System.IO.Packaging;
global using System.Linq;
global using System.Reflection;
#if NET7_0_OR_GREATER
global using System.Text.Json;
#else
//  global using Newtonsoft.Json;
#endif

//global using Qhta.DeepCompare;
global using Qhta.OpenXmlTools;
//global using Qhta.TypeUtils;

global using DX = DocumentFormat.OpenXml;
//global using DXAC = DocumentFormat.OpenXml.AdditionalCharacteristics;
global using DXCP = DocumentFormat.OpenXml.CustomProperties;
global using DXD = DocumentFormat.OpenXml.Drawing;
//global using DXDLCp = DocumentFormat.OpenXml.Drawing.LegacyCompatibility;
//global using DXDLCv = DocumentFormat.OpenXml.Drawing.LockedCanvas;
//global using DXDP = DocumentFormat.OpenXml.Drawing.Pictures;
//using DXDS = DocumentFormat.OpenXml.Drawing.Spreadsheet;
global using DXDW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
//global using DXE = DocumentFormat.OpenXml.EMMA;
global using DXEP = DocumentFormat.OpenXml.ExtendedProperties;
//global using DXFt = DocumentFormat.OpenXml.Features;
//global using DXF = DocumentFormat.OpenXml.Framework;
//global using DXFM = DocumentFormat.OpenXml.Framework.Metadata;
//global using DXFS = DocumentFormat.OpenXml.Framework.Schema;
//global using DXI = DocumentFormat.OpenXml.InkML;
global using DXM = DocumentFormat.OpenXml.Math;
//global using DXOAX = DocumentFormat.OpenXml.Office.ActiveX;
//global using DXOCT = DocumentFormat.OpenXml.Office.ContentType;
//global using DXOCPP = DocumentFormat.OpenXml.Office.CoverPageProps;
//global using DXOCDIP = DocumentFormat.OpenXml.Office.CustomDocumentInformationPanel;
//global using DXOCX = DocumentFormat.OpenXml.Office.CustomXsn;
//global using DXOS = DocumentFormat.OpenXml.Office.Excel;
//global using DXOLP = DocumentFormat.OpenXml.Office.LongProperties;
//global using DXOMA = DocumentFormat.OpenXml.Office.MetaAttributes;
//global using DXOPY21M06M = DocumentFormat.OpenXml.Office.PowerPoint.Y2021.M06.Main;
//global using DXOSMY21EL21 = DocumentFormat.OpenXml.Office.SpreadSheetML.Y2021.ExtLinks2021;
//global using DXOSMY2PVI = DocumentFormat.OpenXml.Office.SpreadSheetML.Y2022.PivotVersionInfo;
//global using DXO10DrawLegComp = DocumentFormat.OpenXml.Office2010.Drawing.LegacyCompatibility;
//global using DXO10DrawPict = DocumentFormat.OpenXml.Office2010.Drawing.Pictures;
//global using DXO10DrawSlicer = DocumentFormat.OpenXml.Office2010.Drawing.Slicer;
//global using DXO10S = DocumentFormat.OpenXml.Office2010.Excel;
//global using DXO10SDraw = DocumentFormat.OpenXml.Office2010.Excel.Drawing;
//global using DXO10SAc = DocumentFormat.OpenXml.Office2010.ExcelAc;
//global using DXO10Ink = DocumentFormat.OpenXml.Office2010.Ink;
//global using DXO10P = DocumentFormat.OpenXml.Office2010.PowerPoint;
global using DXO10W = DocumentFormat.OpenXml.Office2010.Word;
//global using DXO10WCv = DocumentFormat.OpenXml.Office2010.Word.DrawingCanvas;
//global using DXO10WDG = DocumentFormat.OpenXml.Office2010.Word.DrawingGroup;
//global using DXO10WDS = DocumentFormat.OpenXml.Office2010.Word.DrawingShape;
//global using DXO13DTS = DocumentFormat.OpenXml.Office2013.Drawing.TimeSlicer;
//global using DXO13S = DocumentFormat.OpenXml.Office2013.Excel;
//global using DXO13SAc = DocumentFormat.OpenXml.Office2013.ExcelAc;
//global using DXO13P = DocumentFormat.OpenXml.Office2013.PowerPoint;
//global using DXO13PR = DocumentFormat.OpenXml.Office2013.PowerPoint.Roaming;
global using DXO13W = DocumentFormat.OpenXml.Office2013.Word;
//global using DXO16DCAc = DocumentFormat.OpenXml.Office2016.Drawing.ChartsAc;
//global using DXO16DCmd = DocumentFormat.OpenXml.Office2016.Drawing.Command;
//global using DXO16S = DocumentFormat.OpenXml.Office2016.Excel;
//global using DXO16SAc = DocumentFormat.OpenXml.Office2016.ExcelAc;
//global using DXO16P = DocumentFormat.OpenXml.Office2016.Presentation;
//global using DXO16PCmd = DocumentFormat.OpenXml.Office2016.Presentation.Command;
//global using DXO16WSmx = DocumentFormat.OpenXml.Office2016.Word.Symex;
//global using DXO19DA = DocumentFormat.OpenXml.Office2019.Drawing.Animation;
//global using DXO19DA3D = DocumentFormat.OpenXml.Office2019.Drawing.Animation.Model3D;
//global using DXO19DI = DocumentFormat.OpenXml.Office2019.Drawing.Ink;
//global using DXO19DM3D = DocumentFormat.OpenXml.Office2019.Drawing.Model3D;
//global using DXO19SCF = DocumentFormat.OpenXml.Office2019.Excel.CalcFeatures;
//global using DXO19SDA = DocumentFormat.OpenXml.Office2019.Excel.DynamicArray;
//global using DXO19SPDL = DocumentFormat.OpenXml.Office2019.Excel.PivotDefaultLayout;
//global using DXO19SRD = DocumentFormat.OpenXml.Office2019.Excel.RichData;
//global using DXO19SRD2 = DocumentFormat.OpenXml.Office2019.Excel.RichData2;
//global using DXO19STC = DocumentFormat.OpenXml.Office2019.Excel.ThreadedComments;
//global using DXO19P = DocumentFormat.OpenXml.Office2019.Presentation;
//global using DXO21SEL = DocumentFormat.OpenXml.Office2021.Excel.ExternalLinks;
//global using DXO21SNSV = DocumentFormat.OpenXml.Office2021.Excel.NamedSheetViews;
//global using DXO21SP = DocumentFormat.OpenXml.Office2021.Excel.Pivot;
//global using DXO21SRDWI = DocumentFormat.OpenXml.Office2021.Excel.RichDataWebImage;
//global using DXO21SRRVRI = DocumentFormat.OpenXml.Office2021.Excel.RichValueRefreshIntervals;
//global using DXO21STC2 = DocumentFormat.OpenXml.Office2021.Excel.ThreadedComments2;
//global using DXO21PC = DocumentFormat.OpenXml.Office2021.PowerPoint.Comment;
//global using DXO21PD = DocumentFormat.OpenXml.Office2021.PowerPoint.Designer;
//global using DXO21PT = DocumentFormat.OpenXml.Office2021.PowerPoint.Tasks;
global using DXPP = DocumentFormat.OpenXml.Packaging;
//global using DXP = DocumentFormat.OpenXml.Presentation;
//global using DXS = DocumentFormat.OpenXml.Spreadsheet;
//global using DXVl = DocumentFormat.OpenXml.Validation;
//global using DXVlS = DocumentFormat.OpenXml.Validation.Schema;
//global using DXVlSR = DocumentFormat.OpenXml.Validation.Schema.Restrictions;
//global using DXVlS = DocumentFormat.OpenXml.Validation.Semantic;
global using DXVT = DocumentFormat.OpenXml.VariantTypes;
global using DXV = DocumentFormat.OpenXml.Vml;
//global using DXVS = DocumentFormat.OpenXml.Vml.Spreadsheet;
global using DXW = DocumentFormat.OpenXml.Wordprocessing;


