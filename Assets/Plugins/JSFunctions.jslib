var JSFunctions = {
    OnAppReady: function()
    {
        try {
            OnAppReady();
        }
        catch (e) {            
        } 
    },
    SaveSettings: function(str)
    {
        var data = Pointer_stringify(str);
        localStorage.setItem('settings', data);
    },
    ReadSettings: function()
    {        
        var data = localStorage.getItem('settings');
        if((data !== undefined) && (data !== null)){
             //Get size of the string
            var bufferSize = lengthBytesUTF8(data) + 1;
            //Allocate memory space
            var buffer = _malloc(bufferSize);
            //Copy old data to the new one then return it
            stringToUTF8(data, buffer, bufferSize);
            return buffer;
        }
        data = "error";

        var bufferSize = lengthBytesUTF8(data) + 1;
        //Allocate memory space
        var buffer = _malloc(bufferSize);
        //Copy old data to the new one then return it
        stringToUTF8(data, buffer, bufferSize);
        return buffer;
    },
    PrintToConsole: function(str){        
        var data = Pointer_stringify(str);
        console.log(data);
    }
};
mergeInto(LibraryManager.library, JSFunctions);