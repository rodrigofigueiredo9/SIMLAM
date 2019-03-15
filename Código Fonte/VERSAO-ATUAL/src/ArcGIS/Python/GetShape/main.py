import arcpy
import gc
import zipfile
import os
import datetime
import binascii

def timeticks():
    try:
        return str(datetime.datetime.now()).replace("-","").replace(":","").replace(".","").replace(" ","")[:14]
    except Exception:
        raise

try:    
    
    arcpy.AddMessage("Iniciado")

    ids = arcpy.GetParameterAsText(0)
    table = arcpy.GetParameterAsText(1)    

    fileName = timeticks()
    
    TEMP_GEOM = "in_memory\\" + fileName

    path = "C:/Users/userdefault/Desktop/"

    arcpy.AddMessage("Abrindo workspace")

    arcpy.env.workspace = path + "IDAFGEO - M.sde"

    arcpy.MakeFeatureLayer_management("IDAFGEO." + table,TEMP_GEOM, "ID IN (" + ids + ")")

    arcpy.AddMessage("Gerando centroids")

    s_cursor = arcpy.SearchCursor(TEMP_GEOM)
    cursor = s_cursor.next()
    returnCentroid = "["
    
    while cursor:
        print cursor.geometry
        returnCentroid += "{"+str(cursor.geometry.centroid).split()[0].replace(",",".") + "," + str(cursor.geometry.centroid).split()[1].replace(",",".")+"}"
        cursor = s_cursor.next()
        if cursor != None:
            returnCentroid += ","

    returnCentroid += "]"

    arcpy.FeatureClassToShapefile_conversion(TEMP_GEOM, path)

    arcpy.AddMessage("Criando zip")
    
    zf = zipfile.ZipFile(path + fileName + '.zip', mode='w')

    arcpy.AddMessage("Adicionando arquivos")
    
    zf.write(path + fileName + '.shp', fileName + '.shp')
    zf.write(path + fileName + '.dbf', fileName + '.dbf')
    zf.write(path + fileName + '.prj', fileName + '.prj')
    zf.write(path + fileName + '.sbn', fileName + '.sbn')
    zf.write(path + fileName + '.shp.xml', fileName + '.shp.xml')
    zf.write(path + fileName + '.sbx', fileName + '.sbx')
    zf.write(path + fileName + '.shx', fileName + '.shx')
    zf.write(path + fileName + '.cpg', fileName + '.cpg')
    zf.close()

    os.remove(path + fileName + ".shp")
    os.remove(path + fileName + ".dbf")
    os.remove(path + fileName + ".prj")
    os.remove(path + fileName + ".sbn")
    os.remove(path + fileName + ".shp.xml")
    os.remove(path + fileName + ".sbx")
    os.remove(path + fileName + ".shx")
    os.remove(path + fileName + ".cpg")

    in_file = open(path + fileName + ".zip", "rb")
    data = in_file.read()
    in_file.close()

    os.remove(path + fileName + ".zip")
    
    arcpy.SetParameterAsText(2, binascii.b2a_base64(data))
    arcpy.SetParameterAsText(3, returnCentroid)
    
    
except Exception:
    raise
finally:
    del TEMP_GEOM
    gc.collect()
    
