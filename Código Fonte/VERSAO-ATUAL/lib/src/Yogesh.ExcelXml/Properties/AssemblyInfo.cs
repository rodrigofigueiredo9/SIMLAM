using System;
using System.Reflection;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Yogesh.ExcelXml")]
[assembly: AssemblyDescription("Excel import export library")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("Yogesh.ExcelXml")]
[assembly: AssemblyCopyright("Copyright ©  2007-09")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]
[assembly: CLSCompliant(true)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("8491c169-0a55-4d1e-ba9b-47e1030b23bb")]

// Version History (Major only)
// [09/01/2008] Revision 1.00 : Added ExcelXml classes. (Major)
// [19/01/2008] Revision 1.30 : Added DatasetExport (.10)
//								Fixed all FxCop warnings (.10)
//								Added complete documentation. (.10)
// [06/02/2008] Revision 1.41 : Enhanced internal working of Styles class. Almost a rewrite
//								as a new abstract class CellSettingsApplier is added to avoid
//								repeated code. (.10)
//								Added support for stream output. (.01)
// [20/02/2008] Revision 2.43 : Import Facility added. (Major)
//								Cell/Row/Worksheet addition, insertion, deletion added. (Major)
//								Formula and ranges output range on export instead at the time
//								of assignment. (.01)
//								Folder structure and File name changes. (.01)
// [21/02/2008] Revision 2.44 : Fixed a error where assigning 0 to a cell caused a exception.
//								(.01)
// [28/02/2008] Revision 2.45 : Fixed a error where GetValue<T> was not accepting string and
//								DateTime types. (.01)
// [06/03/2008] Revision 2.79 : Added Cell Merge/Unmerge support. (.10)
//								Added GetEnumerator<Cell> support for sheets, rows and 
//								ranges. (.10)
//								Added CellCollection for ranges, sheets and rows. (.10)
//								Fixed a error where numeric output of the cell contained global
//								number format where it should only be US only format. (.01)
//								Added 6 new display format types and removed Custom format
//								type. (.01)
//								Added Index property to cell which also has a ExcelColumnIndex
//								property which returns columns in excel format, eg. A, AA, AC, 
//								FA (.01)
//								CellCompare delegate is now replaced with Predicate<Cell> (.01)
// [10/03/2008] Revision 2.80 : Support for decimal added. (.01)
//								Fixed a error where assigning 0 to a cell still caused a 
//								exception.
// [19/03/2008] Revision 2.81 : Fixed multi worksheet import bug. (.01)
// [22/03/2008] Revision 2.82 : Fixed single cell merge bug. (.01)
// [10/06/2008] Revision 2.89 : Added Print Area support. (.01)
//								Added TabColor property to Worksheet. (.01)
//								Added reference (HRef) support to cell. (.01)
//								Added support for custom display formats. (.01)
//								Added support for cell patterns. (.01)
//								Fixed error where "Long Date", "Short Date", "Time", "@" caused
//								a exception. (.01)
//								Fixed number format issue where some international formats are
//								not saved properly. (.01)
// [23/07/2008] Revision 3.06 : New Formula system implemented. (.10)
//								RowSpan and ColumnSpan properties added to cell. (.01)
//								Small error in named range rename fixed. (.01)
//								Freeze column won't work if freeze row is set. Fixed. (.01)
//								Style was not saved in Column export. Fixed. (.01)
//								Assembly file contained wrong information about this library. 
//								Fixed. (.01)
//								Workbook export can throw a NullReferenceException when new
//								XmlWriter is created. Fixed. (.01)
//								Documented IStyle interfaces. (.01)
[assembly: AssemblyVersion("3.06.614.1455")]
