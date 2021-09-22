import os

SIDE_A_SUBSTRING = '.A'  # Filename contains '.A', ex: Recon.A.png
SIDE_B_SUBSTRING = '.B'  # Filename contains '.B', ex: Recon.B.png


def main():
    """
    This program finds all images in the working directory that don't have a matching Side A and Side B and prints them
    to STDOUT.

    For example, given a directory with following files:
        Recon.A.png
        Recon.B.png
        Tank.A.png
        Tankk.B.png
        SingleSided.png // Ignored
        NonImage.A.txt // Ignored

    It will print:
        ERROR: The following images don't have a matching Side A or Side B:
        Tank.A.png
        Tankk.B.png

        ERROR: The following images don't have a matching Side B:
        Tank.A.png

        ERROR: The following images don't have a matching Side A:
        Tankk.B.png
    """
    current_directory = os.getcwd()

    print(f"Searching for mismatched images in directory: {current_directory}")
    print()

    # Get files in current directory:
    files = []
    for (root, dirs, files) in os.walk(current_directory):
        files = files

    # Get sideA and sideB lists:
    side_a_filenames = [file for file in files if SIDE_A_SUBSTRING in file]
    side_b_filenames = [file for file in files if SIDE_B_SUBSTRING in file]
    map_side_a_name_to_filename = {get_name(side_a_filename, SIDE_A_SUBSTRING): side_a_filename for side_a_filename in side_a_filenames}
    map_side_b_name_to_filename = {get_name(side_b_filename, SIDE_B_SUBSTRING): side_b_filename for side_b_filename in side_b_filenames}

    # Create mapping between Side A to/from Side B:
    map_a_to_b_filenames = {a_filename: map_side_b_name_to_filename.get(a_name) for a_name, a_filename in map_side_a_name_to_filename.items()}
    map_b_to_a_filenames = {b_filename: map_side_a_name_to_filename.get(b_name) for b_name, b_filename in map_side_b_name_to_filename.items()}

    # Print name of all mismatched images:
    side_a_filenames_without_corresponding_side_b = [side_a_filename for side_a_filename, side_b_filename in map_a_to_b_filenames.items() if side_b_filename is None]
    side_b_filenames_without_corresponding_side_a = [side_b_filename for side_b_filename, side_a_filename in map_b_to_a_filenames.items() if side_a_filename is None]

    all_mismatched_filenames = side_a_filenames_without_corresponding_side_b + side_b_filenames_without_corresponding_side_a
    all_mismatched_filenames.sort()

    if len(all_mismatched_filenames) == 0:
        print("SUCCESS: All files have a matching Side A and Side B!")
        print()

    if len(all_mismatched_filenames) > 0:
        print("ERROR: The following images don't have a matching Side A or Side B:")
        for filename in all_mismatched_filenames:
            print(filename)
        print()

    if len(side_a_filenames_without_corresponding_side_b) > 0:
        print("ERROR: The following images don't have a matching Side B:")
        for side_a_filename in side_a_filenames_without_corresponding_side_b:
            print(side_a_filename)
        print()

    if len(side_b_filenames_without_corresponding_side_a) > 0:
        print("ERROR: The following images don't have a matching Side A:")
        for side_b_filename in side_b_filenames_without_corresponding_side_a:
            print(side_b_filename)
        print()


def get_name(filename: str, substring: str) -> str:
    return filename[:filename.find(substring)]


if __name__ == '__main__':
    main()
