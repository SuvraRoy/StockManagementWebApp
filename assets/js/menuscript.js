window.CP.PenTimer.MAX_TIME_IN_LOOP_WO_EXIT = 6000;

// searches next siblings for a target element
const findNextSibling = (elem, selector) => {
  while (elem !== null) {
    elem = elem.nextElementSibling
    if (elem.matches(selector)) return elem
  }
  return elem
}

// if unable to toggle between a and b or b and a
// will return false
const toggleBetween = (elem, a, b) => {
  return elem.classList.replace(a, b) || elem.classList.replace(b, a)
}

// A recursive function to set the total height of an open dropdown
// based on the heights of its child elements
const calcHeights = function (elems) {
  let height = 0
  
  // argument for first call will be a single root element
  // recursive calls will be children
  if (elems instanceof Element) elems = [elems]

  for (const elem of elems) {
    
    // only include calculations for opened dropdowns
    if (elem.matches('.dropdown, .open')) {
      const parentHeight = calcHeights(elem.children)

      if (elem.matches('.open')) {
        elem.style.setProperty('--calc-height', parentHeight + 'px')
      }

      height += parentHeight     
    } else {
      // getBoundingClientRect gives a more accurate height
      const elemHeight = elem.getBoundingClientRect().height
      
      height += (elem.matches('.closed')) ? 0 : elemHeight
    }
  }
  return height
}

const toggleMenu = (dropdown, rootElem) => { 
  if (!toggleBetween(dropdown, 'open', 'closed')) {
    // first time clicking on this dropdown
    dropdown.classList.add('open')
  }
  calcHeights(rootElem)
}

window.addEventListener('DOMContentLoaded', () => {
  const rootElem = document.querySelector('#accordion .dropdown-menu')
  
  // change --calc-height from auto to 0 for initial transition
  document
    .documentElement
    .style.setProperty('--calc-height', '0px')
  
  document
    .querySelectorAll('#accordion .toggle')
    .forEach((toggle) => {
      const dropdown = findNextSibling(toggle, '.dropdown-menu')
      
      toggle.addEventListener('click', (event) => {
        toggleMenu(dropdown, rootElem)
      })
    })
  
    document.querySelector('#toggle-01').click()
})