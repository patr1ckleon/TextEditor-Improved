// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// blob animation removed

// Typing hero animation on Docs Index
(function(){
  const container = document.querySelector('#typing-hero');
  if (!container) return;
  const line = container.querySelector('.line');
  const cursor = container.querySelector('.cursor');
  const text = (line && line.getAttribute('data-text') || '').trim();
  if (!text) return;

  let i = 0; const speed = 70;
  function type(){
    if (i <= text.length){
      line.textContent = text.slice(0, i);
      i++; setTimeout(type, speed);
    } else {
      setTimeout(()=>{
        i = 0; line.textContent = ''; type();
      }, 1200);
    }
  }
  type();

  // blink cursor
  setInterval(()=>{
    if (!cursor) return;
    cursor.style.opacity = cursor.style.opacity === '0' ? '1' : '0';
  }, 400);
})();
