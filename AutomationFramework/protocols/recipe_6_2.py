
from opentrons import protocol_api

metadata = {
    'protocolName': 'Recipe 6.2',
    'author': 'Gruop 662',
}

requirements = {"robotType": "OT-2", "apiLevel": "2.22"}
#Protocol run function
def run(protocol: protocol_api.ProtocolContext):
 # Load labware
    # Load custom labware
 

    cartridge = protocol.load_labware(
        'sbscartridge_20_tuberack_5000ul',  # loadName from your JSON
        '5',                          # Slot number
        label='Custom Tube Rack'
    )
    cartridge.set_offset(x=-33.0, y=-18.0, z=-1.0)
    
    # Resorvoirs
    resorvoir_blue = protocol.load_labware(
        load_name="sbsbath_1_reservoir_300000ul",
        location = 3)
    resorvoir_blue.set_offset(x=0, y=0, z=-3.0)
    
    resorvoir_red = protocol.load_labware( 
        load_name="sbsbath_1_reservoir_300000ul",
        location = 6)
    resorvoir_red.set_offset(x=0, y=0, z=-3.0)
    
    resorvoir_green = protocol.load_labware(
        load_name="sbsbath_1_reservoir_300000ul",
        location= 9)
    resorvoir_green.set_offset(x=0, y=0, z=-3.0)
    
    # Place Tip racks for pipettes
    tiprack_1 = protocol.load_labware(
            load_name="opentrons_96_tiprack_300ul",
            location=11)
    tiprack_1.set_offset(x=1.1, y=-2.8, z=-1.1)

    tiprack_2 = protocol.load_labware(
            load_name="opentrons_96_tiprack_20ul",
            location=10)
    tiprack_2.set_offset(x=1.5, y=-2.5, z=0)
    
    # Load pipettes
    p300 = protocol.load_instrument(
            instrument_name="p300_single_gen2",
            mount="right",
            tip_racks=[tiprack_1])

    p20 = protocol.load_instrument(
            instrument_name="p20_single_gen2",
            mount="left",
           tip_racks=[tiprack_2])
    
    
    p20.transfer(20, cartridge["D5"], cartridge["A2"])

