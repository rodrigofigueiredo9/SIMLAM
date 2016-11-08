package com.tecnomapas.componentes
{
    import mx.controls.DataGrid;
    import mx.controls.dataGridClasses.DataGridColumn;
    import mx.core.mx_internal;
    import mx.events.DataGridEvent;

    use namespace mx_internal;

	public class DataGridCustom extends DataGrid
	{
		private var _savedSortIndex:int = 0;
		private var _savedSortDirection:String = "";
		
		public function DataGridCustom()
		{
			super();
			this.addEventListener(DataGridEvent.HEADER_RELEASE, onHeaderRelease, false, 0);
		}
		
		private function onHeaderRelease(event:DataGridEvent):void
		{
            sortDirection = (sortDirection == "ASC") ? "DESC" : "ASC";

            lastSortIndex = sortIndex;
            sortIndex = event.columnIndex;

            _savedSortIndex = sortIndex;
            _savedSortDirection = sortDirection;
		}
		
		override public function set dataProvider(value:Object):void 
		{
			super.dataProvider = value;
            
            if (_savedSortDirection != null) 
            {
                sortIndex = _savedSortIndex;    
                sortDirection = _savedSortDirection;
            }
        }
        
        public function sortDescending():Boolean
        {
        	return sortDirection == "DESC";
        }
        
	}
}