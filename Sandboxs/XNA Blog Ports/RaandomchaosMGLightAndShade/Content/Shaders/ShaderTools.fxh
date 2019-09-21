float2 RotateRight(float2 coord,float time)
{
	coord.x -= time;
	if(coord.x < 0)	
		coord.x = coord.x+1;
		
	return coord;
}
float2 MoveInCircle(float2 texCoord,float time,float speed)
{
	float2 texRoll = texCoord;
	texRoll.x += cos(time*speed);
	texRoll.y += sin(time*speed);
	
	return texRoll;
}