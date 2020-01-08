var JSFunctions = {
    OnAppReady: function()
    {
      OnAppReady();
    },
    SaveSettings: function(str)
    {
        var data = Pointer_stringify(str);
        localStorage.setItem('settings', data);
    },
    ReadSettings: function(str)
    {        
        var data = localStorage.getItem('settings');
        return data;   
    },
};
mergeInto(LibraryManager.library, JSFunctions);