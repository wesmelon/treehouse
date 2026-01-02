// Utilities for exporting/importing map state
(function () {
  function serialize(state) {
    return JSON.stringify(state, null, 2);
  }

  function download(state, filename = 'farm-state.json') {
    const blob = new Blob([serialize(state)], { type: 'application/json' });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = filename;
    document.body.appendChild(a);
    a.click();
    a.remove();
    URL.revokeObjectURL(url);
  }

  window.MapState = {
    serialize,
    download,
  };
})();
