#version 330

#define ALPHA_OP_AND 0
#define ALPHA_OP_OR 1
#define ALPHA_OP_XOR 2
#define ALPHA_OP_XNOR 3

#define COMP_NEVER 0
#define COMP_LESS 1
#define COMP_EQUAL 2
#define COMP_LEQUAL 3
#define COMP_GREATER 4
#define COMP_NEQUAL 5
#define COMP_GEQUAL 6
#define COMP_ALWAYS 7

uniform int alphaOp;
uniform int alphaComp0;
uniform int alphaComp1;
uniform float alphaRef0;
uniform float alphaRef1;

///
/// preforms gx alpha test
///
bool discard_alpha_test(int comp, float ref, float alpha)
{
	return 
	(
		(comp == COMP_ALWAYS) ||
		(comp == COMP_LESS && alpha < ref) ||
		(comp == COMP_EQUAL && alpha == ref) ||
		(comp == COMP_LEQUAL && alpha <= ref) || 
		(comp == COMP_GREATER && alpha > ref) ||
		(comp == COMP_NEQUAL && alpha != ref) ||
		(comp == COMP_GEQUAL && alpha >= ref)
	);
}

///
/// returns true if fragment should be discarded
///
bool alpha_test(float alpha)
{
	if(alphaOp == -1)
		return false;

	if(alpha < 0)
		return true;
	
	bool ref0 = discard_alpha_test(alphaComp0, alphaRef0, alpha);
	bool ref1 = discard_alpha_test(alphaComp1, alphaRef1, alpha);

	switch(alphaOp)
	{
		case ALPHA_OP_AND:
			if (ref0 && ref1)
				return false;
			break;
		case ALPHA_OP_OR:
			if (ref0 || ref1)
				return false;
			break;
		case ALPHA_OP_XOR:
			if (ref0 != ref1)
				return false;
			break;
		case ALPHA_OP_XNOR:
			if (ref0 == ref1)
				return false;
			break;
	}

	return true;
}