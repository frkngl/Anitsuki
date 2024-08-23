
const togglePositionalHeaderStyles = () => {
    if (window.scrollY > 0 && !header.classList.contains("header-change")) {
        header.classList.add("header-change");
    };
    if (window.scrollY === 0 && header.classList.contains("header-change")) {
        header.classList.remove("header-change");
    };
};

let timeoutID = null;
const header = document.querySelector("header");
togglePositionalHeaderStyles();
window.addEventListener("scroll", () => {
    togglePositionalHeaderStyles();
    if (timeoutID) clearTimeout(timeoutID);
    timeoutID = setTimeout(() => {
        togglePositionalHeaderStyles();
    }, 50);
});

const progressCircle = document.querySelector(".autoplay-progress svg");
const progressContent = document.querySelector(".autoplay-progress span");
var swiper = new Swiper(".mySwiper", {
    spaceBetween: 30,
    loop: true,
    effect: "fade",
    centeredSlides: true,
    autoplay: {
        delay: 4000,
        disableOnInteraction: false
    },
    pagination: {
        el: ".swiper-pagination",
        clickable: true
    },
    navigation: {
        nextEl: ".swiper-button-next",
        prevEl: ".swiper-button-prev"
    },
    on: {
        autoplayTimeLeft(s, time, progress) {
            progressCircle.style.setProperty("--progress", 1 - progress);
            progressContent.textContent = `${Math.ceil(time / 1000)}s`;
        }
    }
});


const video = document.querySelectorAll(".splide__slide .video");
video.forEach(video => {
    video.onmouseenter = () => {
        video.play();
        video.volume = 0.05;
        video.classList.add("active");
    };

    video.onmouseleave = () => {
        video.pause();
        video.classList.remove("active");
    };

    video.ontouchmove = () => {
        video.play();
        video.volume = 0.05;
        video.classList.add("active");
    };

    video.ontouchend = () => {
        video.pause();
        video.classList.remove("active");
    };
});


jQuery.expr[':'].contains = function (a, i, m) {
    return jQuery(a).text().toUpperCase()
        .indexOf(m[3].toUpperCase()) >= 0;
};
$(document).ready(function () {
    $("#searchTags").keyup(function () {
        var value = $("#searchTags").val();
        if (value.length == 0) {
            $("#menuFull li").show();
        } else {
            $("#menuFull li").hide();
            $("#menuFull li:contains(" + value + ")").show();
        }

    });
});

$(document).ready(function () {
    $("#searchTags9").keyup(function () {
        var value = $("#searchTags9").val();
        if (value.length == 0) {
            $("#menuFull9 li").show();
        } else {
            $("#menuFull9 li").hide();
            $("#menuFull9 li:contains(" + value + ")").show();
        }

    });
});










