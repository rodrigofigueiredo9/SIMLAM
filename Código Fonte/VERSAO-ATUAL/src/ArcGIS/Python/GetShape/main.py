import arcgisscripting
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
    gp = arcgisscripting.create(9.3)

    gp.AddMessage("Iniciado")

    ids = gp.GetParameterAsText(0)
    table = gp.GetParameterAsText(1)    

    fileName = timeticks()
    
    TEMP_GEOM = "in_memory\\" + fileName

    path = "D:/arcgisserver/toolboxes/GetShape/"

    gp.AddMessage("Abrindo workspace")
    
    gp.Workspace = path + "IDAFGEO.sde"

    gp.MakeFeatureLayer_management("IDAFGEO." + table,TEMP_GEOM, "ID IN (" + ids + ")")

    gp.AddMessage("Gerando centroids")

    rows = gp.SearchCursor(TEMP_GEOM)

    row = rows.next()

    returnCentroid = "["

    while row:
        returnCentroid += "{"+str(row.geometry.Centroid).split()[0].replace(",",".") + "," + str(row.geometry.Centroid).split()[1].replace(",",".")+"}"
        row = rows.next()
        if row != None:
            returnCentroid += ","

    returnCentroid += "]"

    gp.FeatureClassToShapefile(TEMP_GEOM, path)

    gp.AddMessage("Criando zip")
    
    zf = zipfile.ZipFile(path + fileName + '.zip', mode='w')

    gp.AddMessage("Adicionando arquivos")
    
    zf.write(path + fileName + '.shp', fileName + '.shp')
    zf.write(path + fileName + '.dbf', fileName + '.dbf')
    zf.write(path + fileName + '.prj', fileName + '.prj')
    zf.write(path + fileName + '.sbn', fileName + '.sbn')
    zf.write(path + fileName + '.shp.xml', fileName + '.shp.xml')
    zf.write(path + fileName + '.sbx', fileName + '.sbx')
    zf.write(path + fileName + '.shx', fileName + '.shx')
    zf.close()

    os.remove(path + fileName + ".shp")
    os.remove(path + fileName + ".dbf")
    os.remove(path + fileName + ".prj")
    os.remove(path + fileName + ".sbn")
    os.remove(path + fileName + ".shp.xml")
    os.remove(path + fileName + ".sbx")
    os.remove(path + fileName + ".shx")

    in_file = open(path + fileName + ".zip", "rb")
    data = in_file.read()
    in_file.close()

    os.remove(path + fileName + ".zip")
    
    gp.SetParameterAsText(2, binascii.b2a_base64(data))
    gp.SetParameterAsText(3, returnCentroid)
    
    
except Exception:
    raise
finally:
    del TEMP_GEOM
    gc.collect()
    
