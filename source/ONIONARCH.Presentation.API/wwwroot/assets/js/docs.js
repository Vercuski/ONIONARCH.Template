﻿(function () {
    let link = document.querySelector("link[rel*='icon']") || document.createElement('link');
    document.head.removeChild(link);
    link = document.createElement('link');
    link.type = 'image/x-icon';
    link.rel = 'shortcut icon';
    link.href = '/images/Logo_Leaf_small.png';
    document.getElementsByTagName('head')[0].appendChild(link);
})();