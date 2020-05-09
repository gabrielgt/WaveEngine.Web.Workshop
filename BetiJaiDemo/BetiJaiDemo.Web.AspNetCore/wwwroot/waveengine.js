var Module = typeof Module !== 'undefined' ? Module : {};

Module['locateFile'] = function (base) {
    return `waveengine/${base}`;
}

Module['setProgress'] = function (loadedBytes, totalBytes) {
    let percentage = Math.round((loadedBytes / totalBytes) * 100);
    $('#loading-bar').children().css('width', percentage + '%');

    if (percentage == 100) {
        $('#loading-bar').addClass('progress-infinite');
    }
};

let App = {
    mainCanvasId: undefined,
    configure: function (canvasId, assemblyName, className) {
        this.mainCanvasId = canvasId;
        this.Program.assemblyName = assemblyName;
        this.Program.className = className;
    },
    init: function () {
        this.updateCanvasSize();
        this.Program.Main(this.mainCanvasId);
        window.addEventListener('resize', this.resizeAppSize.bind(this));
    },
    resizeAppSize: function () {
        this.updateCanvasSize();
        this.Program.UpdateCanvasSize(this.mainCanvasId);
    },
    updateCanvasSize: function () {
        let devicePixelRatio = window.devicePixelRatio || 1;
        $(`#${this.mainCanvasId}`)
            .css('width', window.innerWidth + 'px')
            .css('height', window.innerHeight + 'px')
            .prop('width', window.innerWidth * devicePixelRatio)
            .prop('height', window.innerHeight * devicePixelRatio);
    },
    Program: {
        assemblyName: undefined,
        className: undefined,
        Main: function (canvasId) {
            this.invoke('Main', [canvasId]);
        },
        UpdateCanvasSize: function (canvasId) {
            this.invoke('UpdateCanvasSize', [canvasId]);
        },
        invoke: function (methodName, args) {
            BINDING.call_static_method(`[${this.assemblyName}] ${this.className}:${methodName}`, args);
        }
    }
};

let WaveEngine = {
    init: function () {
        $('#splash').fadeOut(function () { $(this).remove(); });
    }
};