// http://eslint.org/docs/user-guide/configuring

module.exports = {
	root: true,
	env: {
		browser: true,
		es6: true,
		jasmine: true
	},
	extends: ['eslint:all', 'plugin:angular/johnpapa'],
	plugins: ['prettier'],
	globals: {
		$: true,
		angular: true,
		pisonApp: true,
		moment: true,
		Enums: true,
		PRODUCTION: true
	},
	parserOptions: {
		ecmaVersion: 2018,
		sourceType: 'module'
	},
	rules: {
		// JavaScript rules
		'array-bracket-spacing': ['error', 'always'],
		'arrow-parens': ['error', 'as-needed'],
		'dot-location': ['error', 'property'],
		indent: ['error', 'tab'],
		'linebreak-style': ['error', 'windows'],
		'lines-around-comment': 'off',
		'max-len': ['error', { code: 120 }],
		'max-statements': 'off',
		'no-extra-parens': [
			'error',
			'all',
			{
				/*
				 * Allow nested expressions
				 * so paranthesis can be used to specify operator precedence between logical operators.
				 */
				nestedBinaryExpressions: false
			}
		],
		'no-magic-numbers': 'off',
		'no-plusplus': 'off',
		'no-tabs': ['error', { allowIndentationTabs: true }],
		'no-ternary': 'off',
		'no-underscore-dangle': ['error', { allow: ['__batchResponses'] }],
		'object-curly-spacing': ['error', 'always'],
		'one-var': ['error', 'never'],
		'padded-blocks': ['error', 'never'],
		'prefer-named-capture-group': 'off',
		'quote-props': ['error', 'as-needed', { numbers: true }],
		quotes: ['error', 'single'],
		semi: ['error', 'always'],
		'sort-imports': 'off',
		'sort-keys': 'off',
		'sort-vars': 'off',
		// AngularJS rules
		'angular/file-name': 'off',
		'angular/function-type': ['error', 'named'],
		'angular/module-getter': 'off',
		'angular/module-setter': 'off',
		'angular/no-service-method': 'off'
	},
	overrides: [
		{
			files: '*.controller.js',
			rules: {
				'consistent-this': ['error', 'vm'],
				// Do not restrict parameter count in AngularJS controller functions;
				// they may have many injected parameters.
				'max-params': 'off'
			}
		},
		// Override rules for AngularJS components
		{
			files: '*.component.js',
			rules: {
				// Do not restrict parameter count in AngularJS directive functions;
				// they may have many injected parameters.
				'max-params': 'off'
			}
		},
		// Override rules for AngularJS directives
		{
			files: '*.directive.js',
			rules: {
				// Do not restrict parameter count in AngularJS directive functions;
				// they may have many injected parameters.
				'max-params': 'off'
			}
		},
		// Override rules for AngularJS services
		{
			files: '*.service.js',
			rules: {
				// Do not restrict parameter count in AngularJS service functions;
				// they may have many injected parameters.
				'max-params': 'off'
			}
		},
		// Override rules for spec files
		{
			files: '*.Spec.js',
			rules: {
				indent: ['error', 2],
				'linebreak-style': ['error', 'windows'],
				'no-tabs': ['error', { allowIndentationTabs: false }]
			}
		}
	]
};
