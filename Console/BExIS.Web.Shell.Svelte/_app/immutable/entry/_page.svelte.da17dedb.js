import {
	S as f,
	i as u,
	s as _,
	k as o,
	q as p,
	l,
	m as h,
	r as x,
	h as r,
	n as g,
	b as v,
	G as d,
	H as i
} from '../chunks/index.615b5812.js';
function b(m) {
	let e, t, n;
	return {
		c() {
			(e = o('div')), (t = o('h1')), (n = p("Let's get cracking bones!")), this.h();
		},
		l(a) {
			e = l(a, 'DIV', { class: !0 });
			var s = h(e);
			t = l(s, 'H1', {});
			var c = h(t);
			(n = x(c, "Let's get cracking bones!")), c.forEach(r), s.forEach(r), this.h();
		},
		h() {
			g(e, 'class', 'container h-full mx-auto flex justify-center items-center');
		},
		m(a, s) {
			v(a, e, s), d(e, t), d(t, n);
		},
		p: i,
		i,
		o: i,
		d(a) {
			a && r(e);
		}
	};
}
class y extends f {
	constructor(e) {
		super(), u(this, e, null, b, _, {});
	}
}
export { y as default };
