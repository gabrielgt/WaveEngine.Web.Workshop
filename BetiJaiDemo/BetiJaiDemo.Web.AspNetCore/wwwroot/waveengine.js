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
    displayZone: function (id) {
        this.Program.DisplayZone(id);

        return false;
    },
    hotspotClicked: function (name) {
        $('#dialog-message').dialog({
            buttons: {
                'Volver al plano': function () {
                    $(this).dialog("close");
                }
            },
            modal: true,
            title: name
        });
    },
    init: function () {
        this.updateCanvasSize();
        this.Program.Main(this.mainCanvasId);
        this.resizeAppSize();
    },
    resizeAppSize: function () {
        this.updateCanvasSize();
        this.Program.UpdateCanvasSize(this.mainCanvasId);
    },
    updateCanvasSize: function () {
        const width = $(`#${this.mainCanvasId}`).width();
        const height = $(`#${this.mainCanvasId}`).height();
        let devicePixelRatio = window.devicePixelRatio || 1;
        $(`#${this.mainCanvasId}`)
            .css('width', width + 'px')
            .css('height', height + 'px')
            .prop('width', width * devicePixelRatio)
            .prop('height', height * devicePixelRatio);
    },
    Program: {
        assemblyName: undefined,
        className: undefined,
        Main: function (canvasId) {
            this.invoke('Main', [canvasId]);
        },
        DisplayZone: function (id) {
            this.invoke('DisplayZone', [id]);
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