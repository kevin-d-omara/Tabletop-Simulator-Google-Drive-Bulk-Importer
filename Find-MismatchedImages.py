import argparse
from dataclasses import dataclass
import os
from typing import List


HIDDEN_FILE_PREFIX = "."
SIDE_A_SUBSTRING = '.A'  # Filename contains '.A', ex: Recon.A.png
SIDE_B_SUBSTRING = '.B'  # Filename contains '.B', ex: Recon.B.png


def main():
    """
    This program finds all images in the specified folder that don't have a matching Side A and Side B and prints them
    to STDOUT.

    Run it with -h to see the usage instructions.

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

        ERROR: The following images don't have a matching Side A:
        Tankk.B.png

        ERROR: The following images don't have a matching Side B:
        Tank.A.png
    """
    # Get program arguments:
    args = ProgramArguments.get_program_arguments()
    print(f"Running script {os.path.basename(__file__)} with arguments:")
    print(args)
    print()

    # Change working directory to the target directory:
    if args.target_directory:
        os.chdir(args.target_directory)

    working_directory = os.getcwd()
    print(f"Starting Directory: {working_directory}")
    print()

    # Recursively walk through all directories.
    # Note: os.walk automatically recurses for us, so we don't need to call it on each subdirectory.
    directories_with_error = []
    for current_directory, subdirectories, files in os.walk(working_directory):
        print(f"Checking filenames in: {current_directory}")

        if not validate_filenames(files):
            directories_with_error.append(current_directory)

        if not args.recursive:
            break

    print()
    num_directories_with_error = len(directories_with_error)
    if num_directories_with_error == 0:
        print(f"SUCCESS SUCCESS SUCCESS ===> There were no errors detected in any directory.")
    else:
        if num_directories_with_error == 1:
            print(f"ERROR ERROR ERROR ===> There was 1 directory with an error. Scroll up to see what the issues were "
                  f"in that directory.")
        else:
            print(f"ERROR ERROR ERROR ===> There were {num_directories_with_error} directories with an error. Scroll "
                  f"up to see what the issues were in each directory.")


@dataclass
class ProgramArguments:
    target_directory: str
    recursive: bool

    @staticmethod
    def get_program_arguments() -> 'ProgramArguments':
        """Get a new instance of ProgramArguments by parsing the program's arguments."""
        parser = ProgramArguments.get_argument_parser()
        args = parser.parse_args()
        return ProgramArguments(**vars(args))

    @staticmethod
    def get_argument_parser() -> argparse.ArgumentParser:
        parser = argparse.ArgumentParser(description='See the script for details.')
        parser.add_argument('-t', '--target-directory', default='', type=str,
                            help='Absolute path to the directory to check filenames in. Defaults to the current '
                                 'directory.')
        parser.add_argument('-r', '--recursive', action='store_true',
                            help='If true, executes recursively inside each folder. Defaults to False.')
        return parser


def validate_filenames(filenames: List[str]) -> bool:
    # Exclude hidden files:
    files = [
        filename for filename
        in filenames
        if not filename.startswith(HIDDEN_FILE_PREFIX)
    ]

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

    are_valid = True

    if len(all_mismatched_filenames) == 0:
        print("SUCCESS: All files have a matching Side A and Side B!")
        print()

    if len(all_mismatched_filenames) > 0:
        are_valid = False
        print("ERROR: The following images don't have a matching Side A or Side B:")
        for filename in all_mismatched_filenames:
            print(filename)
        print()

    if len(side_b_filenames_without_corresponding_side_a) > 0:
        are_valid = False
        print("ERROR: The following images don't have a matching Side A:")
        for side_b_filename in side_b_filenames_without_corresponding_side_a:
            print(side_b_filename)
        print()

    if len(side_a_filenames_without_corresponding_side_b) > 0:
        are_valid = False
        print("ERROR: The following images don't have a matching Side B:")
        for side_a_filename in side_a_filenames_without_corresponding_side_b:
            print(side_a_filename)
        print()

    return are_valid


def get_name(filename: str, substring: str) -> str:
    return filename[:filename.find(substring)]


if __name__ == '__main__':
    main()
