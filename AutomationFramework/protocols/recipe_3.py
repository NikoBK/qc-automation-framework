
from opentrons import protocol_api

metadata = {
    'protocolName': 'Recepi 3',
    'author': 'Gruop 662',
}

requirements = {"robotType": "OT-2", "apiLevel": "2.22"}
#Protocol run function
def run(protocol: protocol_api.ProtocolContext):
 # Load labware
    # Load custom labware
 
    HPLC = protocol.load_labware(
        'sbshplc_20_tuberack_3000ul',  # loadName from your JSON
        '5',                          # Slot number
        label='Custom Tube Rack'
    )
    HPLC.set_offset(x=-32.7, y=-19.2, z=-2.3)
    
    """
    cartridge = protocol.load_labware(
        'sbscartridge_20_tuberack_5000ul',  # loadName from your JSON
        '5',                          # Slot number
        label='Custom Tube Rack'
    )
    cartridge.set_offset(x=3.4, y=2.7, z=-1.2)
    """
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
    
     #actions to be performed
   
    # Aspirate 300uL from A1 and dispense into A4
    p300.transfer(150, HPLC["A1"],HPLC["A2"])
    p20.transfer(20, resorvoir_blue["A1"], HPLC["A2"])
    p300.transfer(200, HPLC["C1"],HPLC["C2"])
    p20.transfer(10, resorvoir_green["A1"], HPLC["C2"])

