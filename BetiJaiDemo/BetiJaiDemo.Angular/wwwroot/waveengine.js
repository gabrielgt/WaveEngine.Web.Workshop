var Module = typeof Module !== 'undefined' ? Module : {};

Module['locateFile'] = function (base) {
    return `waveengine/${base}`;
}

Module['setProgress'] = function (loadedBytes, totalBytes) {
    let percentage = Math.round((loadedBytes / totalBytes) * 100);
    $('#loading-bar').children().css('width', percentage + '%');

    if (percentage === 100) {
        $('#loading-bar').addClass('progress-infinite');
    }
};

let App = {
    mainCanvasId: undefined,
    configure: function (canvasId, assemblyName, className) {
        this.mainCanvasId = canvasId;
        this.Program.assemblyName = assemblyName;
        this.Program.className = className;

        this.bindUI();
    },
    init: function () {
        $('#zones a').first().css('color', '#d3592a');
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
        let canvas = $(`#${this.mainCanvasId}`);
        canvas
            .css('width', window.innerWidth - 400 + 'px')
            .css('height', window.innerHeight - 160 + 'px')
            .prop('width', (window.innerWidth - 400) * devicePixelRatio)
            .prop('height', (window.innerHeight - 160) * devicePixelRatio);
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
        DisplayZone: function(id) {
            this.invoke('DisplayZone', [id]);
        },
        invoke: function (methodName, args) {
            BINDING.call_static_method(`[${this.assemblyName}] ${this.className}:${methodName}`, args);
        }
    },

    // Subscription to dom events
    bindUI: function () {
        $('#zones a').click(function (event) {
            event.preventDefault();

            App.displayZone(this);

            return false;
        });
    },

    // Send events to .Net
    displayZone: function (anchor) {
        $('#zones a').css('color', 'initial');
        $(anchor).css('color', '#d3592a');

        let zoneId = $(anchor).data('zone');
        this.Program.DisplayZone(zoneId);
    },

    // Events received from .Net
    hotspotClicked: function(name) {
        $('#dialog-message').dialog({
            buttons: {
                'Volver al plano': function() {
                    $(this).dialog("close");
                }
            },
            modal: true,
            title: name
        });
    }
};

let WaveEngine = {
    init: function () {
        $('#splash').fadeOut(function () { $(this).remove(); });
    }
};