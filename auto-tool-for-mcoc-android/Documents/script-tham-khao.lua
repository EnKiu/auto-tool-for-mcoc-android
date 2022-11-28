scriptDimension = 1600

Settings:setScriptDimension(true, scriptDimension)

Settings:setCompareDimension(true, scriptDimension)





function checkRecover()

	'''

	Check if a champion needs to be recovered,

	If does - recover.

	Return: True if found, False otherwise

	Rtype : bool

	'''

	reg = Region(375, 291, 150, 150)

	if reg:exists("recover.png") then

		wait(2)

		click(reg:getLastMatch())

		wait(2)

		return true

	end

	return false

end



function verifyRecover()

	'''

	Calling checkRecover() in a loop until

	There are no more champions to recover.

	'''

	res = true

	while res == true do

		res = checkRecover()

		if res == false then

			return

		end

	end

end



function dragChamps()

	'''

	Dragging champions, verifying recovery between 

	Each drag.

	'''

	verifyRecover()

	dragDrop(Location(569, 433), Location(230, 172))

	wait(1)

	verifyRecover()

	dragDrop(Location(569, 433), Location(247, 305))

	wait(1)

	verifyRecover()

	dragDrop(Location(569, 433), Location(226, 428))

end



function clickImage(image, p1, p2, p3, p4)

	'''

	Utility function - clicks on an image.

	Needs 4 rectangle pixels.

	Param image: image to search

	Type image : string 

	Param p1   : first pixel, represent x

	Type p1    : int

	Param p2   : second pixel, represent y

	Type p2    : int

	Param p3   : third pixel, represent h

	Type p3    : int

	Param p4   : fourth pixel, represent w

	Type p4    : int

	Rtype      : bool

	Return     : True if image found in region,

				 False otherwise

	'''

	reg = Region(p1, p2, p3, p4)

	if reg:exists(image) then

		wait(1)

		click(reg:getLastMatch())

		return true

	end



	return false

end



function clickStart(image)

	'''

	Function that used to click on the buttons that

	We need to click in order to start the fight

	Param image: image to click on

	Type  image: string

	Rtype      : bool

	Return     : True if button found and clicked,

				 False otherwise

	'''

	timer = 10

	while timer > 0 do

		res = clickImage(image, 1258, 744, 400, 200)

		if res == true then

			return true

		end

		timer = timer - 1

		wait(1)

	end

	return false

end





function startFight()

	'''

	Starting the fight, clicking on the relevant buttons

	Rtype      : bool

	Return     : True if all buttons found and clicked,

				 False otherwise

	'''

	reg = Region(-45, 736, 400, 200)

	if reg:exists("match.png") then

		click(reg:getLastMatch())

	else

		print("Could not find match.png")

		return false

	end



	res = clickStart("continue.png")

	if res == false then

		print("Could not find continue.png")

		return false

	end



	res = clickStart("accept.png")

	if res == false then

		print("Could not find accept.png")

		return false

	end



	res = clickStart("continue.png")

	if res == false then

		print("Could not find continue.png")

		return false

	end



	return true

end



function fight()

	'''

	Fight function, it assumes that the space button 

	is used for special attack,

	and the right button is used for hitting.

	We can change the time parameter to control

	The time, however in my experience this is

	The best option.

	'''

	time = 20

	while time > 0 do

		--click hit

		click(Location(1286, 577))

		time = time - 0.1

		wait(0.1)

		--click special attack

		click(Location(236, 806))

		wait(0.1)

	end

end



function checkNextFight(image)

	'''

	Checking if we need to proceed to the next fight.

	First check is if you won the first fight,

	if it fails - check if you lost the fight.

	Rtype      : bool

	Return     : True if button found and clicked,

				 False otherwise

	'''

	res = clickImage(image, 761, 607,400, 200)

	if res == false then

		res = clickImage(image, 760, 462, 400, 200)

	end

	return res

end



function run()

	'''

	Main fucntion - executes all the stages

	Rtype      : bool

	Return     : True if all stages went through,

				 False otherwise

	'''

	dragChamps()

	if startFight() == false then

		print("Failed starting fight")

		return false

	end

	wait(5)

	--first fight

	fightEnd = false

	while fightEnd == false do

		fight()

		fightEnd = checkNextFight("next.png")

	end



	--second fight

	fightEnd = false

	while fightEnd == false do

		fight()

		fightEnd = checkNextFight("final.png")

	end



	--third fight

	seriesEnd = false

	while seriesEnd == false do

		fight()

		'''

		If you won at least 1 battle

		'''

		seriesEnd = clickImage("nextSeries.png", 973, 748, 400, 200)

		if seriesEnd == false then

			'''

			If you lost all 3 battles, the screen is different

			'''

			seriesEnd = clickImage("nextSeries.png", 1260, 745, 400, 200)

		end

	end



	return true

end





'''

Due to time restriction by ankulua,

After 30 minutes it will stop,

And if we are in the middle of a fight

We will lose our streak.

So we can control the time by limiting

The runs parameter as we see fit.

'''

runs = 5

totalRuns = runs

while runs > 0 do

	res = run()

	if res == false then

		break

	end

	wait(2)

	runs = runs - 1

end



if runs > 0 then

	print("Failed running")

end
