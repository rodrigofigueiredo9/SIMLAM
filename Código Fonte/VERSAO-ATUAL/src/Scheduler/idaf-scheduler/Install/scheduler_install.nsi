!define PRODUCT_DIR_REGKEY "Software\Microsoft\Windows\CurrentVersion\App Paths\${PRODUCT_EXE}"
!define PRODUCT_PUBLISHER "Tecnomapas"
!define PRODUCT_UNINST_KEY "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}"
!define PRODUCT_UNINST_ROOT_KEY "HKLM"
!define PRODUCT_VERSION "1.0"
!define PRODUCT_WEB_SITE "http://www.tecnomapas.com.br"

; MUI 1.67 compatible ------
!include "MUI2.nsh"
!include "LogicLib.nsh"

; MUI Settings
!define MUI_ABORTWARNING
!define MUI_ICON "${NSISDIR}\Contrib\Graphics\Icons\modern-install.ico"
!define MUI_UNICON "${NSISDIR}\Contrib\Graphics\Icons\modern-uninstall.ico"

; Welcome page
!insertmacro MUI_PAGE_WELCOME
; Directory page
!insertmacro MUI_PAGE_DIRECTORY
; Instfiles page
!insertmacro MUI_PAGE_INSTFILES
; Finish page
!insertmacro MUI_PAGE_FINISH

; Uninstaller pages
!insertmacro MUI_UNPAGE_INSTFILES

; Language files
!insertmacro MUI_LANGUAGE "PortugueseBR"

; MUI end ------

Name "${PRODUCT_NAME} ${PRODUCT_VERSION}"

!ifdef OUTFILE
  OutFile "${OUTFILE}.exe"
!else
  OutFile "NomeDefault.exe"
!endif

InstallDir "$PROGRAMFILES\${PRODUCT_PUBLISHER}\${PRODUCT_DIR}"
InstallDirRegKey HKLM "${PRODUCT_DIR_REGKEY}" ""
ShowInstDetails show
ShowUnInstDetails show
BrandingText "Tecnomapas LTDA."

Section

;---------------------------------------------
  SimpleSC::ExistsService "${SERVICE_NAME}" 
  Pop $0

  StrCmp $0 0 0 endloop
  DetailPrint "${PRODUCT_NAME} is running. Closing it down"
  
  SimpleSC::StopService "${SERVICE_NAME}" 1 30
  Pop $0 ; returns an errorcode (<>0) otherwise success (0)
  
  ; Remove a service
  SimpleSC::RemoveService "${SERVICE_NAME}"

  endloop:
  ;---------------------------------------------

  SetOutPath "$INSTDIR"
  SetOverwrite on
  ;File "${SOURCE_DIR}\${PRODUCT_EXE}"  
  File /r "${SOURCE_DIR}\"
  ; Install a service - ServiceType own process - StartType automatic - NoDependencies - Logon as System Account
  SimpleSC::InstallService "${SERVICE_NAME}" "${PRODUCT_NAME}" "16" "2" "$INSTDIR\${PRODUCT_EXE}" "" "NT AUTHORITY\LocalService" ""
  Pop $0 ; returns an errorcode (<>0) otherwise success (0)  
SectionEnd

Section -Post
  WriteUninstaller "$INSTDIR\uninst.exe"
  WriteRegStr HKLM "${PRODUCT_DIR_REGKEY}" "" "$INSTDIR\${PRODUCT_EXE}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayName" "$(^Name)"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "UninstallString" "$INSTDIR\uninst.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayIcon" "$INSTDIR\${PRODUCT_EXE}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayVersion" "${PRODUCT_VERSION}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "URLInfoAbout" "${PRODUCT_WEB_SITE}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "Publisher" "${PRODUCT_PUBLISHER}"  
  ; Start a service
  ;SimpleSC::StartService "${SERVICE_NAME}" "" 30
  Pop $0 ; returns an errorcode (<>0) otherwise success (0)  
SectionEnd

Function un.onUninstSuccess
  HideWindow
  MessageBox MB_ICONINFORMATION|MB_OK "$(^Name) foi removido com sucesso do seu computador."
FunctionEnd

Function un.onInit
  MessageBox MB_ICONQUESTION|MB_YESNO|MB_DEFBUTTON2 "Tem certeza que quer remover completamente $(^Name) e todos os seus componentes?" IDYES +2
  Abort
FunctionEnd

Section Uninstall
  
  ; Stop a service and waits for file release
  SimpleSC::StopService "${SERVICE_NAME}" 1 30
  Pop $0 ; returns an errorcode (<>0) otherwise success (0)
  
  ; Remove a service
  SimpleSC::RemoveService "${SERVICE_NAME}"
  Pop $0 ; returns an errorcode (<>0) otherwise success (0)
  
  ;Delete "$INSTDIR\uninst.exe"
  ;Delete "$INSTDIR"
  RMDir /r "$INSTDIR"

  DeleteRegKey ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}"
  DeleteRegKey HKLM "${PRODUCT_DIR_REGKEY}"
    
  SetAutoClose true
SectionEnd

