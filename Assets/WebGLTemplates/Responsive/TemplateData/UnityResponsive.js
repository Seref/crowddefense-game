(function () {
    const q = (selector) => document.querySelector(selector);

    const gameContainer = q('#unityContainer');

    const initialDimensions = { width: 1280, height: 720 };
    gameContainer.style.width = '100%';
    gameContainer.style.height = '100%';

    let gCanvasElement = null;

    const getCanvasFromMutationsList = (mutationsList) => {
        for (let mutationItem of mutationsList) {
            for (let addedNode of mutationItem.addedNodes) {
                if (addedNode.id === '#canvas') {
                    return addedNode;
                }
            }
        }
        return null;
    }

    const setDimensions = () => {
        //if (!gCanvasElement.mozFullScreen && !gCanvasElement.webkitIsFullScreen && !gCanvasElement.fullscreen && (gCanvasElement.fullscreenElement == null)) {
        if (!document.fullscreenElement && !document.webkitFullscreenElement &&
            !document.mozFullScreenElement) {
            var winW = parseInt(window.getComputedStyle(gameContainer).width, 10);
            var winH = parseInt(window.getComputedStyle(gameContainer).height, 10);

            var scale = Math.min(winW / initialDimensions.width, winH / initialDimensions.height);
            gCanvasElement.style.width = 'auto';
            gCanvasElement.style.height = 'auto';

            var fitW = Math.round(initialDimensions.width * scale * 100) / 100;
            var fitH = Math.round(initialDimensions.height * scale * 100) / 100;

            gCanvasElement.setAttribute('width', fitW);
            gCanvasElement.setAttribute('height', fitH);
        } else {
            console.log("FullScreen");
        }
    }

    window.setDimensions = setDimensions;

    const registerCanvasWatcher = () => {
        let debounceTimeout = null;

        const debouncedSetDimensions = () => {
            if (debounceTimeout !== null) {
                clearTimeout(debounceTimeout);
            }
            debounceTimeout = setTimeout(setDimensions, 1);
        }
        debouncedSetDimensions();

    }

    window.UnityLoader.Error.handler = function () { }

    const i = 0;
    new MutationObserver(function (mutationsList) {
        const canvas = getCanvasFromMutationsList(mutationsList)
        if (canvas) {
            gCanvasElement = canvas;
            setDimensions();
            new ResizeObserver(registerCanvasWatcher).observe(gameContainer);

            new MutationObserver(function (attributesMutation) {
                this.disconnect();
                setTimeout(registerCanvasWatcher, 1)
            }).observe(canvas, { attributes: true });

            this.disconnect();
        }
    }).observe(gameContainer, { childList: true });

})();