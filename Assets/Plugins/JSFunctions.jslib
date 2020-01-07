var JSFunctions = {
    OnAppReady: function()
    {
      OnAppReady();
    },
    HelloString: function(str)
    {
        window.alert(Pointer_stringify(str));
    },
};
mergeInto(LibraryManager.library, JSFunctions);